namespace MeterReadings.API.Models
{
    /// <summary>
    /// Represents a file of data.
    /// </summary>
    public class SystemFile
    {
        /// <summary>
        /// The file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The contents of the file.
        /// </summary>
        public byte[] FileContents { get; set; }
    }
}