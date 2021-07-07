using MeterReadings.API.Models;
using MeterReadings.Files.ImportResult;
using MeterReadings.Files.MeterReadings;
using System.Web.Http;

namespace MeterReadings.API.Controllers
{
    public class MeterReadingController : ApiController
    {
        [Route("meter-reading-uploads")]
        [HttpPost]
        public FileImportResult UploadMeterReadFile(SystemFile file)
        {
            return new MeterReadingsFile(file.FileName, file.FileContents).ImportFromFile();
        }
    }
}