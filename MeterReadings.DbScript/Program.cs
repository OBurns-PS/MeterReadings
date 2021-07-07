using Crudinski.Tools.DbScript.Db;
using MeterReadings.Logic.Providers;
using MeterReadings.Model;
using System.Data;

namespace MeterReadings.DbScript
{
    /// <summary>
    /// 
    /// USAGE:
    /// Call this command line app, passing the connection string as the first argument.
    /// Run this after creating the DB on chosen DB server (this is currently geared up for T-SQL) to create tables and seed database with data.
    /// 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            using (IDbConnection connection = ConnectionProvider.Instance.GetConnection(args[0]))
            {
                MeterReadingsDataHouse dataHouse = new MeterReadingsDataHouse();
                DbScriptDatabaseUpdater updater = new DbScriptDatabaseUpdater(connection, dataHouse, ParserProvider.Instance, DbFunctionParserProvider.Instance);

                connection.Open();
                updater.ApplyUpdate(ScriptMasterList.MasterList);
                connection.Close();
            }
        }
    }
}
