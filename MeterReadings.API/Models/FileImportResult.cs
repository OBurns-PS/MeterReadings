namespace MeterReadings.API.Models
{
    /// <summary>
    /// Contains details and results of a file import.
    /// </summary>
    public class FileImportResult
    {
        /// <summary>
        /// The number of records that successfully imported.
        /// </summary>
        public int SuccessCount { get; }

        /// <summary>
        /// The number of records that failed to import.
        /// </summary>
        public int FailureCount { get; }

        /// <summary>
        /// If the file contains "duplicate" records, the number of records identified as such.
        /// </summary>
        public int FileDuplicates { get; }

        /// <summary>
        /// The messages associated with the file import.
        /// </summary>
        public string[] FailureMessages { get; }

        private FileImportResult(int successCount, int failureCount, int duplicateCount, params string[] failureMessages)
        {
            SuccessCount = successCount;
            FailureCount = failureCount;
            FileDuplicates = duplicateCount;
            FailureMessages = failureMessages;
        }

        /// <summary>
        /// Converts a file import process to an API FileImportResult.
        /// </summary>
        /// <param name="fileImportResult">The inbound FileImportResult to convert.</param>
        public static explicit operator FileImportResult(Files.ImportResult.FileImportResult fileImportResult)
        {
            return new FileImportResult(
                fileImportResult.SuccessCount,
                fileImportResult.FailureCount,
                fileImportResult.FileDuplicates,
                fileImportResult.FailureMessages);
        }
    }
}