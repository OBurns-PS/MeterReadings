using BRepo.Files.Attributes.Validation;
using BRepo.Files.Interface;
using BRepo.Files.Reporting.Validation.Types;
using System.Runtime.CompilerServices;

namespace MeterReadings.Files.MeterReadings
{
    public class ValidMeterReadingLengthAttribute : ValidationBase
    {
        public ValidMeterReadingLengthAttribute([CallerMemberName] string propertyName = "") 
            : base(ValidationLevels.Error, propertyName)
        { }

        public override bool IsValid(string value, IFileComponent fileComponent)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            return value.Length.Equals(_meterReadingLength);
        }

        protected override string GetValidationFailureMessage(int lineNumber)
        {
            return $"Meter reading length was incorrect. A meter reading must be {_meterReadingLength} digits.";
        }

        private static readonly int _meterReadingLength = 5;
    }
}
