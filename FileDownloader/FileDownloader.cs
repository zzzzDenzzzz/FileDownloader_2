using System.Net.Http.Headers;

namespace FileDownloader
{
    public class FileDownloader
    {
        public static async Task DownloadFileAsync(string url,
            string filePath, CancellationToken cancellationToken,
            int bufferSize)
        {
            PathCheck(url);
            PathCheck(filePath);

            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(url,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? -1L;
            var isResuming = false;
            var bytesDownloaded = 0L;

            if (FileExistenceCheck(response, filePath))
            {
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length < totalBytes)
                {
                    httpClient.DefaultRequestHeaders.Range = new RangeHeaderValue(fileInfo.Length, null);
                    isResuming = true;
                    bytesDownloaded = fileInfo.Length;
                }
            }

            await WriteToFileOutputToConsole(response, filePath, isResuming,
                bufferSize, cancellationToken, bytesDownloaded, totalBytes);
        }

        private static void PathCheck(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"{nameof(path)} is null or empty.", nameof(path));
            }
        }

        private static bool FileExistenceCheck(HttpResponseMessage response, string filePath)
        {
            if (response.Headers.AcceptRanges.Contains("bytes") && File.Exists(filePath))
            {
                return true;
            }
            return false;
        }

        private static async Task WriteToFileOutputToConsole(HttpResponseMessage response,
            string filePath, bool isResuming, int bufferSize, CancellationToken cancellationToken,
            long bytesDownloaded, long totalBytes)
        {
            using var stream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(filePath,
                isResuming ? FileMode.Append : FileMode.Create,
                FileAccess.Write,
                FileShare.None);
            var buffer = new byte[bufferSize];
            var bytesRead = 0;
            var lastProgressPercentage = 0;

            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                bytesDownloaded += bytesRead;

                var progressPercentage = (int)((double)bytesDownloaded / totalBytes * 100);
                if (progressPercentage != lastProgressPercentage)
                {
                    Console.WriteLine($"Downloaded {bytesDownloaded} bytes out of {totalBytes} ({progressPercentage}%)");
                    lastProgressPercentage = progressPercentage;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }
    }
}
