using BRepo.Files.Attributes.Validation;
using BRepo.Files.Interface;
using BRepo.Files.Reporting.Validation.Types;
using System.Runtime.CompilerServices;

namespace MeterReadings.Files.MeterReadings
{
    public class ValidMeterReadingValueAttribute : ValidationBase
    {
        public ValidMeterReadingValueAttribute([CallerMemberName] string propertyName = "") 
            : base(ValidationLevels.Error, propertyName)
        { }

        public override bool IsValid(string value, IFileComponent fileComponent)
        {
            foreach(char c in value)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }

        protected override string GetValidationFailureMessage(int lineNumber)
        {
            return "Meter reading can only contain numbers (0-9).";
        }
    }
}
