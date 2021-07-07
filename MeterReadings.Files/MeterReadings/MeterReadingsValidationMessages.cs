namespace MeterReadings.Files.MeterReadings
{
    public static class MeterReadingsValidationMessages
    {
        public static string GetDuplicateMeterReadingFile(int accountID) 
        {
            return $"Warning: Duplicate Meter reading was removed (AccountID: {accountID}). " +
                $"The same reading was submitted twice on the same day.";
        }
    }
}
