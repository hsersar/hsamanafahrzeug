using Microsoft.Extensions.Logging;

namespace FahrzeugZulassung.Infrastructure.FileStorage;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<Stream?> GetFileAsync(string filePath, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);
    Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default);
    Task<long> GetFileSizeAsync(string filePath, CancellationToken cancellationToken = default);
}

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _baseStoragePath;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(string baseStoragePath, ILogger<LocalFileStorageService> logger)
    {
        _baseStoragePath = baseStoragePath ?? throw new ArgumentNullException(nameof(baseStoragePath));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Ensure base storage directory exists
        if (!Directory.Exists(_baseStoragePath))
        {
            Directory.CreateDirectory(_baseStoragePath);
            _logger.LogInformation("Created base storage directory: {Path}", _baseStoragePath);
        }
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        if (fileStream == null)
            throw new ArgumentNullException(nameof(fileStream));

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be empty", nameof(fileName));

        try
        {
            // Create a unique subdirectory based on date and GUID
            var dateFolder = DateTime.UtcNow.ToString("yyyy/MM/dd");
            var uniqueFolder = Guid.NewGuid().ToString();
            var relativePath = Path.Combine(dateFolder, uniqueFolder);
            var fullPath = Path.Combine(_baseStoragePath, relativePath);

            // Ensure directory exists
            Directory.CreateDirectory(fullPath);

            // Sanitize file name
            var sanitizedFileName = SanitizeFileName(fileName);
            var filePath = Path.Combine(fullPath, sanitizedFileName);

            // Save file
            using (var fileStreamOut = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
            {
                await fileStream.CopyToAsync(fileStreamOut, cancellationToken);
            }

            var relativeFilePath = Path.Combine(relativePath, sanitizedFileName);
            _logger.LogInformation("File saved successfully: {Path}", relativeFilePath);

            return relativeFilePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file: {FileName}", fileName);
            throw;
        }
    }

    public async Task<Stream?> GetFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty", nameof(filePath));

        try
        {
            var fullPath = Path.Combine(_baseStoragePath, filePath);

            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("File not found: {Path}", filePath);
                return null;
            }

            var memoryStream = new MemoryStream();
            using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
            {
                await fileStream.CopyToAsync(memoryStream, cancellationToken);
            }

            memoryStream.Position = 0;
            return memoryStream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving file: {Path}", filePath);
            throw;
        }
    }

    public Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty", nameof(filePath));

        try
        {
            var fullPath = Path.Combine(_baseStoragePath, filePath);

            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("File not found for deletion: {Path}", filePath);
                return Task.FromResult(false);
            }

            File.Delete(fullPath);
            _logger.LogInformation("File deleted successfully: {Path}", filePath);

            // Try to delete empty parent directories
            TryDeleteEmptyDirectories(Path.GetDirectoryName(fullPath));

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {Path}", filePath);
            throw;
        }
    }

    public Task<bool> FileExistsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return Task.FromResult(false);

        var fullPath = Path.Combine(_baseStoragePath, filePath);
        return Task.FromResult(File.Exists(fullPath));
    }

    public Task<long> GetFileSizeAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty", nameof(filePath));

        var fullPath = Path.Combine(_baseStoragePath, filePath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        var fileInfo = new FileInfo(fullPath);
        return Task.FromResult(fileInfo.Length);
    }

    private static string SanitizeFileName(string fileName)
    {
        // Remove invalid characters from file name
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        
        // Ensure the file name is not too long (max 255 characters)
        if (sanitized.Length > 255)
        {
            var extension = Path.GetExtension(sanitized);
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(sanitized);
            sanitized = nameWithoutExtension.Substring(0, 255 - extension.Length) + extension;
        }

        return sanitized;
    }

    private void TryDeleteEmptyDirectories(string? directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
            return;

        try
        {
            // Don't delete the base storage path
            if (directoryPath == _baseStoragePath || !directoryPath.StartsWith(_baseStoragePath))
                return;

            if (Directory.Exists(directoryPath) && !Directory.EnumerateFileSystemEntries(directoryPath).Any())
            {
                Directory.Delete(directoryPath);
                _logger.LogDebug("Deleted empty directory: {Path}", directoryPath);

                // Recursively delete parent directories if they are empty
                TryDeleteEmptyDirectories(Path.GetDirectoryName(directoryPath));
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not delete directory: {Path}", directoryPath);
        }
    }
}
