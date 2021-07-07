namespace MeterReadings.API.Models
{
    public class SystemFile
    {
        public string FileName { get; set; }
        public byte[] FileContents { get; set; }
    }
}