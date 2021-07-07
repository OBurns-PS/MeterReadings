using MeterReadings.API.Models;
using MeterReadings.Files.MeterReadings;
using System.Web.Http;

namespace MeterReadings.API.Controllers
{
    /// <summary>
    /// Controls Actions around submissions of meter readings.
    /// </summary>
    public class MeterReadingController : ApiController
    {
        /// <summary>
        /// Submits a file containing Meter readings into the system.
        /// </summary>
        /// <param name="file">The file containing the meter readings to submit.</param>
        /// <returns>A report on how many items successfully imported, how many were rejected, how many duplicate readings were identified and any associated 
        /// import error / warning messages.</returns>
        [Route("meter-reading-uploads")]
        [HttpPost]
        public FileImportResult UploadMeterReadFile(SystemFile file)
        {
            var fileImportResult = new MeterReadingsFile(file.FileName, file.FileContents).ImportFromFile();
            return (FileImportResult)fileImportResult;
        }
    }
}