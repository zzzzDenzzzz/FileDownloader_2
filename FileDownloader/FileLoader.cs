namespace FileDownloader
{
    public class FileLoader
    {
        private readonly HttpClient _httpClient;

        public FileLoader(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SaveFileAsAsync(string url, string filePath)
        {
            using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();

            using var fileStream = new FileStream(
                filePath, 
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                8192,
                true);

            await stream.CopyToAsync(fileStream);
        }
    }
}
