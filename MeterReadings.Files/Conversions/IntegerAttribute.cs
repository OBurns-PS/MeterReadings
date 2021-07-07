using BRepo.Files.Attributes.Conversion;
using BRepo.Files.Interface;
using BRepo.Files.Reporting.Validation.Types;
using System;
using System.Runtime.CompilerServices;

namespace MeterReadings.Files.Conversions
{
    /// <summary>
    /// Converts a value to a 32-bit integer.
    /// </summary>
    public class IntegerAttribute : CustomValueConversionBase
    {
        public IntegerAttribute([CallerMemberName] string propertyName = "") 
            : base(ValidationLevels.Error, propertyName)
        { }

        public override bool Convert(string value, IFileComponent fileComponent, out IConvertible convertedValue)
        {
            convertedValue = default(int);
            try
            {
                convertedValue = System.Convert.ToInt32(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override string ConvertBack(IConvertible value, IFileComponent fileComponent)
        {
            return value.ToString();
        }

        protected override string GetErrorMessage()
        {
            return "Invalid integer encountered on conversion.";
        }
    }
}
