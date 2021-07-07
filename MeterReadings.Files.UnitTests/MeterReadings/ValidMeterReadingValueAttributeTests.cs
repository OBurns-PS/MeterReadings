using BRepo.Files.Interface;
using FluentAssertions;
using MeterReadings.Files.MeterReadings;
using Moq;
using NUnit.Framework;

namespace MeterReadings.Files.UnitTests.MeterReadings
{
    public class ValidMeterReadingValueAttributeTests
    {
        [TestCase("01002", true)]
        [TestCase("45345", true)]
        [TestCase("2001", true)]
        [TestCase("-06575", false)]
        [TestCase("VOID", false)]
        [TestCase("0X765", false)]
        public void ShouldValueBeTreatedCorrectly(string value, bool expectedResult)
        {
            ValidMeterReadingValueAttribute attribute = new ValidMeterReadingValueAttribute();
            Mock<IFileComponent> mockComponent = new Mock<IFileComponent>();
            attribute.IsValid(value, mockComponent.Object).Should().Be(expectedResult);
        }
    }
}
