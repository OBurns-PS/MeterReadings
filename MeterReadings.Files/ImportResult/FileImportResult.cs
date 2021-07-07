namespace MeterReadings.Files.ImportResult
{
    public class FileImportResult
    {
        public int SuccessCount { get; }
        public int FailureCount { get; }
        public int FileDuplicates { get; }
        public string[] FailureMessages { get; }
        
        public FileImportResult(int successCount, int failureCount, int duplicateCount, params string[] failureMessages)
        {
            SuccessCount = successCount;
            FailureCount = failureCount;
            FileDuplicates = duplicateCount;
            FailureMessages = failureMessages;
        }
    }
}
