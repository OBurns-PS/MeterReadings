using BRepo.Files.Interface;
using FluentAssertions;
using MeterReadings.Files.MeterReadings;
using Moq;
using NUnit.Framework;

namespace MeterReadings.Files.UnitTests.MeterReadings
{
    public class ValidMeterReadingLengthAttributeTests
    {
        [TestCase("00478", true)]
        [TestCase("999999", false)]
        [TestCase("-06575", false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        public void ShouldValueBeTreatedCorrectly(string value, bool expectedResult)
        {
            ValidMeterReadingLengthAttribute attribute = new ValidMeterReadingLengthAttribute();
            Mock<IFileComponent> mockComponent = new Mock<IFileComponent>();
            attribute.IsValid(value, mockComponent.Object).Should().Be(expectedResult);
        }
    }
}
