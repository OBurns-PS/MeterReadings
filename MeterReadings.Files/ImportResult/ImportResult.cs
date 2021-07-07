namespace MeterReadings.Files.ImportResult
{
    public class ImportResult
    {
        public int LineNumber { get; }

        public ImportStatus ImportStatus { get; }

        public string[] ImportMessages { get; }

        public ImportResult(int lineNumber, ImportStatus importStatus, params string[] importMessages)
        {
            LineNumber = lineNumber;
            ImportStatus = importStatus;
            ImportMessages = importMessages;
        }
    }
}
