using FluentAssertions;
using MeterReadings.Files.MeterReadings;
using NUnit.Framework;
using System;

namespace MeterReadings.Files.UnitTests.MeterReadings
{
    public class MeterReadingsRecordTests
    {
        [Test]
        public void ShouldSameDayReadingNotBeEqual()
        {
            MeterReadingsRecord firstReading = new MeterReadingsRecord()
            {
                AccountID = 1234,
                MeterReadingDateTime = DateTime.Today,
                MeterReadValue = 1007
            };

            MeterReadingsRecord secondReading = new MeterReadingsRecord()
            {
                AccountID = 1234,
                MeterReadingDateTime = DateTime.Now,
                MeterReadValue = 1007
            };

            firstReading.Equals(secondReading).Should().Be(true);
        }

        [Test]
        public void ShouldDifferentDaySameReadingNotBeEqual()
        {
            MeterReadingsRecord firstReading = new MeterReadingsRecord()
            {
                AccountID = 1234,
                MeterReadingDateTime = DateTime.Now,
                MeterReadValue = 1007
            };

            MeterReadingsRecord secondReading = new MeterReadingsRecord()
            {
                AccountID = 1234,
                MeterReadingDateTime = DateTime.Now.AddDays(1),
                MeterReadValue = 1009
            };

            firstReading.Equals(secondReading).Should().Be(false);
        }
    }
}
