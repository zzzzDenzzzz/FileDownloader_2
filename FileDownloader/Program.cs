using FileDownloader;

var loader = new FileLoader(new HttpClient());
await loader.SaveFileAsAsync(
    "https://github.com/rodion-m/SystemProgrammingCourse2022/raw/master/files/payments_19mb.zip",
    "file.zip");

/*
 * В данной реализации полученные байты записываются в файл по мере скачивания,
 * используя FileStream с FileMode.Append или FileMode.Create,
 * в зависимости от того, является ли закачка продолжением предыдущей.
 * 
 * Добавлен функционал отмены операции через CancellationToken.
 * 
 * Выводится прогресс закачки в консоль.
 * 
 * Реализован функционал докачки файлов при помощи заголовка Range.
 * Если сервер поддерживает докачку, и файл уже существует,
 * то добавляется заголовок Range с указанием количества уже загруженных байтов,
 * и закачка продолжается с этого места.
 */

await FileDownloader.FileDownloader.DownloadFileAsync(
    "https://github.com/rodion-m/SystemProgrammingCourse2022/raw/master/files/payments_19mb.zip",
    "file_1.zip", new CancellationToken(), 4096);
