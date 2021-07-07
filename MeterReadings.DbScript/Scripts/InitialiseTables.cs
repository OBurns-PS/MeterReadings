using Crudinski.Tools.DbScript.Db;
using Crudinski.Tools.DbScript.Model.Interface;
using MeterReadings.Model.Objects;

namespace MeterReadings.DbScript.Scripts
{
    public class InitialiseTables : IDbScript
    {
        public string ScriptName => nameof(InitialiseTables);

        public int ScriptVersion => 1;

        public void Apply(DbScriptDatabaseUpdater updater)
        {
            updater.TableFunctions.CreateTableIfNotExists<Account>();
            updater.TableFunctions.CreateTableIfNotExists<Person>();
            updater.TableFunctions.CreateTableIfNotExists<MeterReading>();
        }
    }
}
