using BRepo.Files.Interface;
using FluentAssertions;
using MeterReadings.Files.Conversions;
using Moq;
using NUnit.Framework;
using System;

namespace MeterReadings.Files.UnitTests.Conversions
{
    public class IntegerAttributeTests
    {
        [TestCase("45114", true, 45114)]
        [TestCase("999999", true, 999999)]
        [TestCase(" 0415", true, 415)]
        [TestCase("00943", true, 943)]
        [TestCase("VOID", false, 0)]
        [TestCase("0x332", false, 0)]
        public void ShouldConvertReturnCorrectly(string inputValue, bool expectedResult, IConvertible expectedConversionValue)
        {
            IntegerAttribute attribute = new IntegerAttribute();
            Mock<IFileComponent> mockComponent = new Mock<IFileComponent>();

            bool result = attribute.Convert(inputValue, mockComponent.Object, out IConvertible output);

            result.Should().Be(expectedResult);
            output.Should().Be(expectedConversionValue);
        }
    }
}
