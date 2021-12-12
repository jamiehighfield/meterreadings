namespace MeterReadings
{
    /// <summary>
    /// Used to store files uploaded and stored in memory into temporary file storage.
    /// </summary>
    public class TemporaryFileUploadHandler
    {
        /// <summary>
        /// Handles an uploaded file from an instance of <see cref="IFormFile"/> and stores the uploaded file in temporary file storage, returning an instance of <see cref="UploadedFile"/> that can be used to interface with the file.
        /// </summary>
        /// <param name="file">The file that has been uploaded.</param>
        /// <returns>An instance of <see cref="UploadedFile"/>.</returns>
        public async Task<UploadedFile> HandleUploadAsync(IFormFile file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var temporaryFile = Path.GetTempFileName();

            using (Stream fileStream = File.Create(temporaryFile))
            {
                await file.CopyToAsync(fileStream);
            }

            return new UploadedFile(temporaryFile);
        }
    }

    /// <summary>
    /// Represents a file uploaded and stored in temporary file storage.
    /// </summary>
    public class UploadedFile : IDisposable
    {
        /// <summary>
        /// Initialise a new instance of <see cref="UploadedFile"/>.
        /// </summary>
        /// <param name="filePath">The temporary path of the file.</param>
        public UploadedFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException($"'{nameof(filePath)}' cannot be null or empty.", nameof(filePath));
            }

            _filePath = filePath;
        }

        private string _filePath;
        private Stream _stream;

        /// <summary>
        /// Opens a pointer to the file as a stream.
        /// </summary>
        /// <returns>An instance of <see cref="Stream"/> pointing to the start of the file.</returns>
        public Stream Open()
        {
            _stream = File.Open(_filePath, FileMode.Open);

            return _stream;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            if (_stream != null) _stream.Dispose();

            if (File.Exists(_filePath)) File.Delete(_filePath);
        }
    }
}