using Crudinski.Tools.DbScript.Db;
using Crudinski.Tools.DbScript.Interface;
using Crudinski.Tools.DbScript.Tsql.Model;

namespace MeterReadings.DbScript
{
    public class DbFunctionParserProvider : IDbFunctionParserProvider
    {
        public static DbFunctionParserProvider Instance
        {
            get
            {
                return new DbFunctionParserProvider();
            }
        }

        public IDbFunctionParser GetParser(DbScriptDatabaseUpdater updater)
        {
            return new TsqlDbFunctionParser(updater);
        }
    }
}
