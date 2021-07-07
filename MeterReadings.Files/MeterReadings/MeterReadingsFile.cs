using BRepo.Files.FileTypes;
using BRepo.Files.FileTypes.Standard;
using BRepo.Files.Reporting.Validation;
using BRepo.Files.Reporting.Validation.Types;
using MeterReadings.Files.ImportResult;
using MeterReadings.Logic.Collections;
using MeterReadings.Logic.Records;
using System.Collections.Generic;
using System.Linq;

namespace MeterReadings.Files.MeterReadings
{
    public class MeterReadingsFile 
        : DelimitedFile<IgnoreHeader, MeterReadingsRecord, IgnoreFooter>
    {
        public MeterReadingsFile(string fileName, byte[] fileContents) 
            : base(fileName, fileContents)
        { }

        public override bool ValidateFileName()
        {
            return true;
        }

        public static FileImportResult ImportFromFile(string fileName, byte[] fileArray)
        {
            MeterReadingsFile file = new MeterReadingsFile(fileName, fileArray);
            FileObject<IgnoreHeader, MeterReadingsRecord, IgnoreFooter> importObject = file.Parse();

            List<string> failureMessages = new List<string>();
            int successCount = importObject.ValidDetails.Count();
            int failureCount = importObject.ItemErrors;
            foreach(ValidationMessage failure in importObject.ValidationMessages
                .Where(x => x.FileSection == FileSection.Item))
            {
                failureMessages.Add($"Line {failure.LineNumber}: {failure.Message}");
            }

            var uniqueReadings = new HashSet<MeterReadingsRecord>();
            int duplicateItems = 0;
            foreach (MeterReadingsRecord meterReading in importObject.ValidDetails)
            {
                if (!uniqueReadings.Add(meterReading))
                {
                    failureMessages.Add(MeterReadingsValidationMessages.GetDuplicateMeterReadingFile(meterReading.AccountID));
                    duplicateItems++;
                    successCount--;
                    failureCount++;
                }
            }

            SubmitMeterReadings(ConvertMeterReadings(uniqueReadings), out List<string> importValidations);
            successCount -= importValidations.Count;
            failureCount += importValidations.Count;
            
            failureMessages.AddRange(importValidations);
            return new FileImportResult(successCount, failureCount, duplicateItems, failureMessages.ToArray());
        }

        private static void SubmitMeterReadings(IEnumerable<MeterReading> meterReadings, out List<string> validationMessages)
        {
            AccountCollection accounts = new AccountCollection();
            accounts.SubmitMeterReadings(meterReadings, out validationMessages);
        }

        private static IEnumerable<MeterReading> ConvertMeterReadings(IEnumerable<MeterReadingsRecord> fileRecords)
        {
            return fileRecords.Select(x => new MeterReading()
            {
                AccountID = x.AccountID,
                MeterReadingDateTime = x.MeterReadingDateTime,
                MeterReadValue = x.MeterReadValue
            });
        }
    }
}
