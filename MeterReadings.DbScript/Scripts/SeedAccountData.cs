using Crudinski.Entity.Logic.Filters;
using Crudinski.Tools.DbScript.Db;
using Crudinski.Tools.DbScript.Model.Interface;
using MeterReadings.Logic.Collections;
using MeterReadings.Logic.Records;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MeterReadings.DbScript.Scripts
{
    public class SeedAccountData : IDbScript
    {
        public string ScriptName => nameof(SeedAccountData);

        public int ScriptVersion => 1;

        public void Apply(DbScriptDatabaseUpdater updater)
        {
            AccountCollection seedAccountData = new AccountCollection();

            updater.DatabaseFunctions.ExecuteProcedure(GetIdentityInsertCommand(updater, true));
            SetExampleAccountNumbers();
            IEnumerable<IDbCommand> accountInsertCommands = _seedData.Select(x => GetAccountInsertCommand(updater, x));

            foreach(IDbCommand insertCommand in accountInsertCommands)
            {
                updater.DatabaseFunctions.ExecuteProcedure(insertCommand);
            }

            updater.DatabaseFunctions.ExecuteProcedure(GetIdentityInsertCommand(updater, false));

            seedAccountData.AccountIDs = new IsInFilter<int>(true);
            seedAccountData.AccountIDs.AddRange(_seedData.Select(x => x.AccountID));
            seedAccountData.Read(updater.Connection, updater.Transaction, updater.ParserProvider);

            foreach(Account account in seedAccountData)
            {
                List<Model.Objects.Person> persons = GetSeedPersons(account.AccountID);
                account.Person.AddRange(persons);
            }
            seedAccountData.Save(updater.Connection, updater.Transaction, updater.ParserProvider);
        }

        private static readonly List<Account> _seedData = new List<Account>()
        {
            new Account()
            {
                AccountID = 2344,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 2344,
                        FirstName = "Tommy",
                        LastName = "Test"
                    }
                }
            },
            new Account()
            {
                AccountID = 2233,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 2233,
                        FirstName = "Barry",
                        LastName = "Test"
                    }
                }
            },
            new Account()
            {
                AccountID = 8766,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 8766,
                        FirstName = "Sally",
                        LastName = "Test"
                    }
                }
            },
            new Account()
            {
                AccountID = 2345,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 2345,
                        FirstName = "Jerry",
                        LastName = "Test"
                    }
                }
            },            
            new Account()
            {
                AccountID = 2346,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 2346,
                        FirstName = "Ollie",
                        LastName = "Test"
                    }
                }
            },
            new Account()
            {
                AccountID = 2347,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 2347,
                        FirstName = "Tara",
                        LastName = "Test"
                    }
                }
            },            
            new Account()
            {
                AccountID = 2348,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 2348,
                        FirstName = "Tammy",
                        LastName = "Test"
                    }
                }
            },            
            new Account()
            {
                AccountID = 2349,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 2349,
                        FirstName = "Simon",
                        LastName = "Test"
                    }
                }
            },            
            new Account()
            {
                AccountID = 2350,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 2350,
                        FirstName = "Colin",
                        LastName = "Test"
                    }
                }
            },            
            new Account()
            {
                AccountID = 2351,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 2351,
                        FirstName = "Gladys",
                        LastName = "Test"
                    }
                }
            },            
            new Account()
            {
                AccountID = 2352,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 2352,
                        FirstName = "Greg",
                        LastName = "Test"
                    }
                }
            },            
            new Account()
            {
                AccountID = 2353,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 2353,
                        FirstName = "Tony",
                        LastName = "Test"
                    }
                }
            },            
            new Account()
            {
                AccountID = 2355,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 2355,
                        FirstName = "Arthur",
                        LastName = "Test"
                    }
                }
            },            
            new Account()
            {
                AccountID = 2356,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 2356,
                        FirstName = "Craig",
                        LastName = "Test"
                    }
                }
            },            
            new Account()
            {
                AccountID = 6776,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 6776,
                        FirstName = "Laura",
                        LastName = "Test"
                    }
                }
            },            
            new Account()
            {
                AccountID = 4534,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 4534,
                        FirstName = "JOSH",
                        LastName = "TEST"
                    }
                }
            },            
            new Account()
            {
                AccountID = 1234,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 1234,
                        FirstName = "Freya",
                        LastName = "Test"
                    }
                }
            },            
            new Account()
            {
                AccountID = 1239,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 1239,
                        FirstName = "Noddy",
                        LastName = "Test"
                    }
                }
            },            
            new Account()
            {
                AccountID = 1240,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 1240,
                        FirstName = "Archie",
                        LastName = "Test"
                    }
                }
            },            
            new Account()
            {
                AccountID = 1241,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 1241,
                        FirstName = "Lara",
                        LastName = "Test"
                    }
                }
            },            
            new Account()
            {
                AccountID = 1242,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 1242,
                        FirstName = "Tim",
                        LastName = "Test"
                    }
                }
            },            
            new Account()
            {
                AccountID = 1243,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 1243,
                        FirstName = "Graham",
                        LastName = "Test"
                    }
                }
            },            
            new Account()
            {
                AccountID = 1244,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 1244,
                        FirstName = "Tony",
                        LastName = "Test"
                    }
                }
            },            
            new Account()
            {
                AccountID = 1245,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 1245,
                        FirstName = "Neville",
                        LastName = "Test"
                    }
                }
            },            
            new Account()
            {
                AccountID = 1246,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 1246,
                        FirstName = "Jo",
                        LastName = "Test"
                    }
                }
            },            
            new Account()
            {
                AccountID = 1247,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 1247,
                        FirstName = "Jim",
                        LastName = "Test"
                    }
                }
            },            
            new Account()
            {
                AccountID = 1248,
                Person = new List<Model.Objects.Person>()
                {
                    new Person()
                    {
                        AccountID = 1248,
                        FirstName = "Pam",
                        LastName = "Test"
                    }
                }
            }
        };

        private IDbCommand GetIdentityInsertCommand(DbScriptDatabaseUpdater updater, bool isOn)
        {
            IDbCommand command = updater.Connection.CreateCommand();
            command.Transaction = updater.Transaction;

            command.CommandType = CommandType.Text;
            command.CommandText = $"SET IDENTITY_INSERT {nameof(Model.Objects.Account)} {(isOn ? "ON" : "OFF")}";
            return command;
        }

        private IDbCommand GetAccountInsertCommand(DbScriptDatabaseUpdater updater, Account account)
        {
            IDbCommand command = updater.Connection.CreateCommand();
            command.Transaction = updater.Transaction;

            command.CommandType = CommandType.Text;
            command.CommandText = "INSERT INTO Account(AccountID, AccountNumber) VALUES(@p0, @p1)";

            IDbDataParameter accountID = command.CreateParameter();
            accountID.ParameterName = "@p0";
            accountID.Value = account.AccountID;
            command.Parameters.Add(accountID);

            IDbDataParameter accountNumber = command.CreateParameter();
            accountNumber.ParameterName = "@p1";
            accountNumber.Value = account.AccountNumber;
            command.Parameters.Add(accountNumber);

            return command;
        }

        private List<Model.Objects.Person> GetSeedPersons(int accountID)
        {
            Account matchingAccount = _seedData.First(x => x.AccountID == accountID);
            return matchingAccount.Person;
        }

        private void SetExampleAccountNumbers()
        {
            for (int i = 0; i < _seedData.Count; i++)
            {
                _seedData[i].AccountNumber = $"TestAccount{(i + 1).ToString().PadLeft(3, '0')}";
            }
        }
    }
}
