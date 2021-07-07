using FluentAssertions;
using MeterReadings.Files.ImportResult;
using MeterReadings.Files.MeterReadings;
using MeterReadings.Logic.Interface;
using MeterReadings.Logic.Records;
using MeterReadings.UnitTestHelpers.Helpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeterReadings.Files.UnitTests.MeterReadings
{
    public class MeterReadingsFileTests
    {
        [SetUp]
        public void Init()
        {
            _entityToDataReader = new EntityToDataReader(true);
            _entityToDataReader.Command.Setup(m => m.ExecuteScalar()).Returns(1);
            Mock<IConnectionProvider> mockConnectionProvider = new Mock<IConnectionProvider>();
            mockConnectionProvider.Setup(m => m.GetConnection()).Returns(_entityToDataReader.Connection.Object);

            _mockConnectionProvider = mockConnectionProvider.Object;
            _meterReadingsFile = new MeterReadingsFile(_fileName, new byte[0], _mockConnectionProvider);
        }

        [Test]
        public void ShouldValidateFileNameReturnCorrectValue()
        {
            _meterReadingsFile.ValidateFileName().Should().BeTrue();
        }

        [TestCase("8766,22/04/2019 12:25,03440", "2351,22/04/2019 12:25,57579", "1238,16/05/2019 09:24,00000")]
        public void ShouldValidFileLoadCorrectly(params string[] fileLines)
        {
            _meterReadingsFile = new MeterReadingsFile(_fileName, CreateTestFile(fileLines), _mockConnectionProvider);
            CreateMockDbAccounts(fileLines);
            FileImportResult result = _meterReadingsFile.ImportFromFile();

            result.FailureCount.Should().Be(0);
            result.SuccessCount.Should().Be(3);
        }

        [TestCase("8766,2019/04/22 12:25,03440")]
        public void ShouldFileRecordDateInvalidFormatRaiseError(params string[] fileLines)
        {
            _meterReadingsFile = new MeterReadingsFile(_fileName, CreateTestFile(fileLines), _mockConnectionProvider);
            CreateMockDbAccounts(fileLines);
            FileImportResult result = _meterReadingsFile.ImportFromFile();

            result.SuccessCount.Should().Be(0);
            result.FailureCount.Should().Be(1);

            result.FailureMessages.Should().ContainSingle();
            result.FailureMessages[0].Should().Be("Line 2: Value is not a valid Date Format (dd/MM/yyyy HH:mm).");
        }

        [TestCase("2346,22/04/2019 12:25,999999")]
        public void ShouldFileRecordInvalidMeterReadingLengthRaiseError(params string[] fileLines)
        {
            _meterReadingsFile = new MeterReadingsFile(_fileName, CreateTestFile(fileLines), _mockConnectionProvider);
            CreateMockDbAccounts(fileLines);
            FileImportResult result = _meterReadingsFile.ImportFromFile();

            result.SuccessCount.Should().Be(0);
            result.FailureCount.Should().Be(1);
            result.FailureMessages.Should().ContainSingle();

            result.FailureMessages.Should().Contain("Line 2: File Item: Error validating field MeterReadValue. Meter reading length was incorrect. " +
                "A meter reading must be 5 digits.");
        }

        [TestCase("2344,08/05/2019 09:24,0X765")]
        public void ShouldFileRecordInvalidMeterReadingValueRaiseError(params string[] fileLines)
        {
            _meterReadingsFile = new MeterReadingsFile(_fileName, CreateTestFile(fileLines), _mockConnectionProvider);
            CreateMockDbAccounts(fileLines);
            FileImportResult result = _meterReadingsFile.ImportFromFile();

            result.SuccessCount.Should().Be(0);
            result.FailureCount.Should().Be(1);
            result.FailureMessages.Should().HaveCount(2);

            result.FailureMessages.Should().Contain("Line 2: File Item: Error validating field MeterReadValue. Meter reading can only contain numbers (0-9).");
        }

        [TestCase("2344,08/05/2019 09:24,0X765")]
        public void ShouldFileRecordNonIntegerFailConversion(params string[] fileLines)
        {
            _meterReadingsFile = new MeterReadingsFile(_fileName, CreateTestFile(fileLines), _mockConnectionProvider);
            CreateMockDbAccounts(fileLines);
            FileImportResult result = _meterReadingsFile.ImportFromFile();

            result.SuccessCount.Should().Be(0);
            result.FailureCount.Should().Be(1);
            result.FailureMessages.Should().HaveCount(2);

            result.FailureMessages.Should().Contain("Line 2: Invalid integer encountered on conversion.");
        }

        [TestCase("8766,22/04/2019 12:25,03440", "8766,22/04/2019 18:34,03440")]
        public void ShouldReadingsFileDuplicateMeterReadingSameDayDedupe(params string[] fileLines)
        {
            _meterReadingsFile = new MeterReadingsFile(_fileName, CreateTestFile(fileLines), _mockConnectionProvider);
            CreateMockDbAccounts(fileLines);
            FileImportResult result = _meterReadingsFile.ImportFromFile();

            result.SuccessCount.Should().Be(1);
            result.FailureCount.Should().Be(1);
            result.FailureMessages.Should().ContainSingle();

            result.FailureMessages[0].Should().Be("Warning: Duplicate Meter reading was removed (AccountID: 8766). " +
                "The same reading was submitted twice on the same day.");
        }

        [TestCase("8766,22/04/2019 12:25,03440", "8766,22/04/2019 18:34,03447")]
        public void ShouldReadingsFileDifferentReadingSameDayNotDedupe(params string[] fileLines)
        {
            _meterReadingsFile = new MeterReadingsFile(_fileName, CreateTestFile(fileLines), _mockConnectionProvider);
            CreateMockDbAccounts(fileLines);
            FileImportResult result = _meterReadingsFile.ImportFromFile();

            result.SuccessCount.Should().Be(2);
            result.FailureCount.Should().Be(0);
            result.FailureMessages.Should().BeEmpty();
        }

        private static byte[] CreateTestFile(IEnumerable<string> fileLines)
        {
            StringBuilder file = new StringBuilder();

            file.AppendLine("AccountId,MeterReadingDateTime,MeterReadValue");
            foreach(string fileLine in fileLines)
            {
                file.AppendLine(fileLine);
            }
            file.AppendLine(string.Empty);
            return Encoding.UTF8.GetBytes(file.ToString());
        }

        private void CreateMockDbAccounts(params string[] fileLines)
        {
            List<Account> mockAccounts = new List<Account>();
            foreach(string line in fileLines)
            {
                mockAccounts.Add(new Account()
                {
                    AccountID = ExtractAccountID(line),
                    AccountNumber = string.Empty
                });
            }
            _entityToDataReader.SetupMockResponse((string x) => x.Contains("FROM [Account]"), mockAccounts);
        }

        private int ExtractAccountID(string fileLine)
        {
            return Convert.ToInt32(fileLine.Split(',')[0]);
        }

        private EntityToDataReader _entityToDataReader;
        private MeterReadingsFile _meterReadingsFile;
        private IConnectionProvider _mockConnectionProvider;

        private static readonly string _fileName = "TestFile.csv";
    }
}
