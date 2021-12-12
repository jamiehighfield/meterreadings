using System.Collections.Concurrent;
using System.Reflection;

namespace MeterReadings.DataAccess
{
    /// <summary>
    /// Helper class to read embedded resources.
    /// </summary>
    internal static class EmbeddedFile
    {
        private static ConcurrentDictionary<string, string> _fileCache = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Reads a file as an embedded resource and returns the string representation of that file.
        /// </summary>
        /// <param name="fileName">The file to read.</param>
        /// <param name="cache">A <see cref="bool"/> value indicating whether or not the file contents should be cached.</param>
        /// <returns>The string representation of the read file.</returns>
        internal static string Read(string fileName, bool cache = true)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException($"'{nameof(fileName)}' cannot be null or empty.", nameof(fileName));
            }

            return _fileCache.GetOrAdd(fileName, (key) =>
            {
                Assembly executingAssembly = Assembly.GetExecutingAssembly();
                var resourceName = $"MeterReadings.DataAccess.{key}";

                using (Stream resourceStream = executingAssembly.GetManifestResourceStream(resourceName))
                {
                    if (resourceStream is null)
                    {
                        throw new ArgumentException($"Could not find file '{resourceName}' as an embedded resource.");
                    }

                    using (StreamReader fileReader = new StreamReader(resourceStream))
                    {
                        string fileContents = fileReader.ReadToEnd();

                        if (cache)
                        {
                            _fileCache[key] = fileContents;
                        }

                        return fileContents;
                    }
                }
            });
        }
    }
}