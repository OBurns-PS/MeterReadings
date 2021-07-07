using FluentAssertions;
using MeterReadings.Logic.Collections;
using MeterReadings.Logic.Interface;
using MeterReadings.Logic.Records;
using MeterReadings.UnitTestHelpers.Helpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MeterReadings.Logic.UnitTests.Collections
{
    public class AccountCollectionTests
    {
        [SetUp]
        public void Init()
        {
            _entityToDataReader = new EntityToDataReader(true);
            _entityToDataReader.Command.Setup(m => m.ExecuteScalar()).Returns(() => _internalIDCounter++);

            Mock<IConnectionProvider> mockConnectionProvider = new Mock<IConnectionProvider>();
            mockConnectionProvider.Setup(m => m.GetConnection()).Returns(_entityToDataReader.Connection.Object);
            
            _connectionProvider = mockConnectionProvider.Object;
            _accountCollection = new AccountCollection(_connectionProvider);
        }

        [Test]
        public void ShouldMatchingAccountsValidNewMeterReadingsSubmitWithoutErrors()
        {
            List<MeterReading> meterReadings = new List<MeterReading>()
            {
                new MeterReading()
                {
                    AccountID = 1234,
                    MeterReadingDateTime = DateTime.Now,
                    MeterReadValue = 12345
                },
                new MeterReading()
                {
                    AccountID = 5678,
                    MeterReadingDateTime = DateTime.Now,
                    MeterReadValue = 67894
                }
            };

            List<Account> dbAccounts = new List<Account>()
            {
                new Account()
                {
                    AccountID = 1234,
                    AccountNumber = "TestAccount01"
                },
                new Account()
                {
                    AccountID = 5678,
                    AccountNumber = "TestAccount02"
                }
            };

            _entityToDataReader.SetupMockResponse((string s) => s.Contains("FROM [Account]"), dbAccounts);
            _accountCollection.SubmitMeterReadings(meterReadings, out List<string> validationMessages);
            validationMessages.Should().BeEmpty();

            _accountCollection.Should().HaveCount(2);

            foreach(Account account in _accountCollection)
            {
                account.MeterReading.Should().ContainSingle();
                account.MeterReading[0].MeterReadingID.Should().BeGreaterThan(-1);
            }
        }

        [Test]
        public void ShouldNoMatchingAccountValidateProperly()
        {
            List<MeterReading> meterReadings = new List<MeterReading>()
            {
                new MeterReading()
                {
                    AccountID = 2234,
                    MeterReadingDateTime = DateTime.Now,
                    MeterReadValue = 12345
                }
            };

            List<Account> dbAccounts = new List<Account>()
            {
                new Account()
                {
                    AccountID = 1234,
                    AccountNumber = "TestAccount01"
                },
                new Account()
                {
                    AccountID = 5678,
                    AccountNumber = "TestAccount02"
                }
            };

            _entityToDataReader.SetupMockResponse((string s) => s.Contains("FROM [Account]"), dbAccounts);
            _accountCollection.SubmitMeterReadings(meterReadings, out List<string> validationMessages);
            validationMessages.Should().ContainSingle();

            validationMessages[0].Should().Be("Meter reading submitted for a non-existent account (Account ID: 2234)");

            _accountCollection.Should().HaveCount(2);

            foreach (Account account in _accountCollection)
            {
                account.MeterReading.Should().BeEmpty();
            }
        }

        [Test]
        public void ShouldDuplicateDbMeterReadingValidateProperly()
        {
            List<MeterReading> meterReadings = new List<MeterReading>()
            {
                new MeterReading()
                {
                    AccountID = 1234,
                    MeterReadingDateTime = DateTime.Now,
                    MeterReadValue = 12345
                }
            };

            List<Account> dbAccounts = new List<Account>()
            {
                new Account()
                {
                    AccountID = 1234,
                    AccountNumber = "TestAccount01"
                }
            };

            List<Model.Objects.MeterReading> dbMeterReadings = new List<Model.Objects.MeterReading>()
            {
                new Model.Objects.MeterReading()
                {
                    AccountID = 1234,
                    MeterReadingDateTime = DateTime.Now,
                    MeterReadValue = 12345
                }
            };

            _entityToDataReader.SetupMockResponse((string s) => s.Contains("FROM [Account]"), dbAccounts);
            _entityToDataReader.SetupMockResponse((string s) => s.Contains("FROM [MeterReading]"), dbMeterReadings);
            _accountCollection.SubmitMeterReadings(meterReadings, out List<string> validationMessages);
            validationMessages.Should().ContainSingle();

            validationMessages[0].Should().Be("Duplicate meter reading (same day and same value) submitted for Account ID 1234.");

            _accountCollection.Should().ContainSingle();
            _accountCollection[0].MeterReading.Should().ContainSingle();
        }

        private IConnectionProvider _connectionProvider;
        private EntityToDataReader _entityToDataReader = new EntityToDataReader();
        private AccountCollection _accountCollection;

        private static int _internalIDCounter = 1;
    }
}
