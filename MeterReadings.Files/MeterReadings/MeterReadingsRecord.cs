using BRepo.Files.Attributes.Conversion;
using BRepo.Files.Attributes.Structure;
using BRepo.Files.Interface;
using MeterReadings.Files.Conversions;
using System;

namespace MeterReadings.Files.MeterReadings
{
    /// <summary>
    /// A meter reading file record.
    /// </summary>
    public class MeterReadingsRecord : IFileDetails
    {
        [FieldOrder(1)]
        public int AccountID { get; set; }

        [FieldOrder(2)]
        [DateFormat("dd/MM/yyyy HH:mm")]
        public DateTime MeterReadingDateTime { get; set; }

        [FieldOrder(3)]
        [ValidMeterReadingLength]
        [ValidMeterReadingValue]
        [Integer]
        public int MeterReadValue { get; set; }
        
        public bool IsValid { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is MeterReadingsRecord meterReadingsRecord)
            {
                return Equals(meterReadingsRecord);
            }
            return base.Equals(obj);
        }

        public bool Equals(MeterReadingsRecord meterReadingsRecord)
        {
            return AccountID.Equals(meterReadingsRecord.AccountID)
                     && MeterReadingDateTime.Date.Ticks.Equals(meterReadingsRecord.MeterReadingDateTime.Date.Ticks)
                     && MeterReadValue.Equals(meterReadingsRecord.MeterReadValue);
        }

        public override int GetHashCode()
        {
            return AccountID.GetHashCode()
                * MeterReadingDateTime.Date.Ticks.GetHashCode()
                * MeterReadValue.GetHashCode();
        }
    }
}
