using Crudinski.Entity.Logic.Attributes;
using Crudinski.Entity.Logic.Filters;
using MeterReadings.Logic.Interface;
using MeterReadings.Logic.Providers;
using MeterReadings.Logic.Records;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MeterReadings.Logic.Collections
{
    public class AccountCollection : MeterReadingCollection<Account>
    {
        [Filter(nameof(Account.AccountID), false)]
        public IsInFilter<int> AccountIDs { get; set; }

        public AccountCollection()
            : this(ConnectionProvider.Instance)
        { }

        public AccountCollection(IConnectionProvider connectionProvider)
            : base()
        {
            _connectionProvider = connectionProvider;
        }

        /// <summary>
        /// Validates meter readings and, if valid, posts them to the DB.
        /// </summary>
        /// <param name="meterReadings">The meter readings to validation / post.</param>
        /// <param name="validationMessages">Any validation messages raised as a result of posting the readings.</param>
        public void SubmitMeterReadings(IEnumerable<MeterReading> meterReadings, out List<string> validationMessages)
        {
            List<string> validations = new List<string>();
            AccountIDs = new IsInFilter<int>(true);
            AccountIDs.AddRange(meterReadings.Select(x => x.AccountID));
            DataHouse.IncludeRelationship<Model.Objects.Account, Model.Objects.MeterReading>();

            ReadMeterReadingCollection(this, _connectionProvider);
            List<int> missingAccountIDs = GetMissingAccountIDs(
                this, meterReadings.ToList(), out List<string> duplicateMessages);

            validations.AddRange(duplicateMessages);

            IEnumerable<MeterReading> matchingMeterReadings = meterReadings
                .Where(x => !missingAccountIDs.Contains(x.AccountID));

            List<int> duplicateMeterReadingIDs = GetDuplicateMeterReadingIDs(this, matchingMeterReadings, out duplicateMessages, 
                out Dictionary<Account, List<MeterReading>> newMeterReadings);

            validations.AddRange(duplicateMessages);

            IEnumerable<MeterReading> validMeterReadings = matchingMeterReadings
                .Where(x => !duplicateMeterReadingIDs.Contains(x.MeterReadingID));

            PostMeterReadings(this, newMeterReadings);

            validationMessages = validations;
        }

        private static List<int> GetMissingAccountIDs(AccountCollection accounts, List<MeterReading> meterReadings, out List<string> duplicateMessages)
        {
            HashSet<int> missingAccountIDs = new HashSet<int>();
            duplicateMessages = new List<string>();

            for (int i = meterReadings.Count - 1; i >= 0; i--)
            {
                MeterReading meterReading = meterReadings[i];
                Account matchingAccount = accounts.FirstOrDefault(x => x.AccountID == meterReading.AccountID);
                if (matchingAccount == null)
                {
                    duplicateMessages.Add(GetMissingAccountMessage(meterReading.AccountID));
                    missingAccountIDs.Add(meterReading.AccountID);
                }
            }
            return missingAccountIDs.ToList();
        }

        private static List<int> GetDuplicateMeterReadingIDs(AccountCollection accounts, IEnumerable<MeterReading> meterReadings, out List<string> duplicateMessages,
            out Dictionary<Account, List<MeterReading>> newMeterReadings)
        {
            newMeterReadings = new Dictionary<Account, List<MeterReading>>();
            HashSet<int> duplicateReadingIDs = new HashSet<int>();
            duplicateMessages = new List<string>();

            foreach(MeterReading meterReading in meterReadings)
            {
                Account matchingAccount = accounts.First(x => x.AccountID == meterReading.AccountID);
                if (matchingAccount.MeterReading.Any(
                        x => x.MeterReadingDateTime.Date.Ticks == meterReading.MeterReadingDateTime.Date.Ticks
                            && x.MeterReadValue == meterReading.MeterReadValue))
                {
                    duplicateReadingIDs.Add(meterReading.MeterReadingID);
                    duplicateMessages.Add(GetDuplicateMeterReadingMessage(meterReading.AccountID));
                }
                else
                {
                    if (!newMeterReadings.ContainsKey(matchingAccount))
                    {
                        newMeterReadings[matchingAccount] = new List<MeterReading>();
                    }
                    newMeterReadings[matchingAccount].Add(meterReading);
                }
            }
            return duplicateReadingIDs.ToList();
        }

        private void PostMeterReadings(AccountCollection accounts, Dictionary<Account, List<MeterReading>> newMeterReadings)
        {
            foreach(var newMeterReadingAccount in newMeterReadings)
            {
                newMeterReadingAccount.Key.MeterReading.AddRange(newMeterReadingAccount.Value);
            }

            using (IDbConnection connection = _connectionProvider.GetConnection())
            {
                try
                {
                    connection.Open();
                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            accounts.Save(connection, transaction, ParserProvider.Instance);
                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                        }
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private static string GetMissingAccountMessage(int accountID)
        {
            return $"Meter reading submitted for a non-existent account (Account ID: {accountID})";
        }

        private static string GetDuplicateMeterReadingMessage(int accountID)
        {
            return $"Duplicate meter reading (same day and same value) submitted for Account ID {accountID}.";
        }

        private readonly IConnectionProvider _connectionProvider;
    }
}
