namespace AllupWebApplication.Helpers.Extensions;

public static class FileManager
{
    // Adds asynchronous operation for saving files
    public static async Task<string> SaveFileAsync(this IFormFile file, string rootPath, string folderName)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty or null.", nameof(file));

        // Ensure the file is an image
        if (!file.ContentType.StartsWith("image/"))
            throw new ArgumentException("File is not an image.", nameof(file));

        // Truncate the file name if it exceeds 64 characters and prepend a GUID
        string fileName = Path.GetFileName(file.FileName);
        fileName = fileName.Length > 64 ? fileName.Substring(fileName.Length - 64, 64) : fileName;
        fileName = Guid.NewGuid() + "_" + fileName; // Ensures uniqueness

        string path = Path.Combine(rootPath, folderName, fileName);

        // Creates the directory if it doesn't exist
        Directory.CreateDirectory(Path.Combine(rootPath, folderName));

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return fileName;
    }

    public static void DeleteFile(string rootPath, string folderName, string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return;

        string deletePath = Path.Combine(rootPath, folderName, fileName);
        if (File.Exists(deletePath))
        {
            File.Delete(deletePath);
        }
    }
}