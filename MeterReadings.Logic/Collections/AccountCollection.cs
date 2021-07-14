using Crudinski.Entity.Logic.Attributes;
using Crudinski.Entity.Logic.Context;
using Crudinski.Entity.Logic.Filters;
using Crudinski.Entity.Logic.Lists;
using MeterReadings.Logic.Interface;
using MeterReadings.Logic.Providers;
using MeterReadings.Model;
using MeterReadings.Model.Objects;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MeterReadings.Logic.Collections
{
    public class AccountCollection : DataItemList<Account>, IAccountCollection
    {
        public override DataHouse DataHouse { get; } = new MeterReadingsDataHouse();

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
        public void SubmitMeterReadings(List<MeterReading> meterReadings, out List<string> validationMessages)
        {
            validationMessages = new List<string>();
            GetAccounts(meterReadings.Select(x => x.AccountID));

            ValidateMissingAccountIds(this, meterReadings, validationMessages);

            Dictionary<Account, List<MeterReading>> validAccountMeterReadings 
                = GetValidDedupeMeterReadings(this, meterReadings, validationMessages);

            AddMeterReadingsToAccounts(validAccountMeterReadings);
            SubmitAccountReadings();
        }

        private void ValidateMissingAccountIds(AccountCollection accounts, List<MeterReading> meterReadings, List<string> validationMessages)
        {
            HashSet<int> missingAccountIDs = new HashSet<int>();

            for (int i = meterReadings.Count - 1; i >= 0; i--)
            {
                MeterReading meterReading = meterReadings[i];
                Account matchingAccount = accounts.FirstOrDefault(x => x.AccountID == meterReading.AccountID);
                if (matchingAccount == null)
                {
                    validationMessages.Add(GetMissingAccountMessage(meterReading.AccountID));
                    missingAccountIDs.Add(meterReading.AccountID);
                }
            }
            meterReadings.RemoveAll(x => missingAccountIDs.Contains(x.AccountID));
        }

        private static Dictionary<Account, List<MeterReading>> GetValidDedupeMeterReadings(
            AccountCollection accounts, List<MeterReading> meterReadings, List<string> validationMessages)
        {
            Dictionary<Account, List<MeterReading>> result 
                = new Dictionary<Account, List<MeterReading>>();

            foreach(MeterReading meterReading in meterReadings)
            {
                CheckForDuplicateReading(accounts, validationMessages, result, meterReading);
            }
            return result;
        }

        private static void CheckForDuplicateReading(AccountCollection accounts, List<string> duplicateMessages, 
            Dictionary<Account, List<MeterReading>> newMeterReadings, MeterReading meterReading)
        {
            Account matchingAccount = accounts.First(x => x.AccountID == meterReading.AccountID);
            if (matchingAccount.MeterReading.Any(
                    x => x.MeterReadingDateTime.Date.Ticks == meterReading.MeterReadingDateTime.Date.Ticks
                        && x.MeterReadValue == meterReading.MeterReadValue))
            {
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

        public List<Account> GetAccounts(IEnumerable<int> accountIds)
        {
            AccountIDs = new IsInFilter<int>(true);
            DataHouse.IncludeRelationship<Account, MeterReading>();
            AccountIDs.AddRange(accountIds);
            using (IDbConnection connection = _connectionProvider.GetConnection())
            {
                connection.Open();
                Read(connection, null, ParserProvider.Instance);
                connection.Close();
            }
            return this.ToList();
        }

        public void SubmitAccountReadings()
        {
            using (IDbConnection connection = _connectionProvider.GetConnection())
            {
                try
                {
                    connection.Open();
                    using (IDbTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            Save(connection, transaction, ParserProvider.Instance);
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                    connection.Close();
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private static void AddMeterReadingsToAccounts(Dictionary<Account, List<MeterReading>> validAccountMeterReadings)
        {
            foreach(KeyValuePair<Account, List<MeterReading>> accountMeterReadings in validAccountMeterReadings)
            {
                accountMeterReadings.Key.MeterReading.AddRange(accountMeterReadings.Value);
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
