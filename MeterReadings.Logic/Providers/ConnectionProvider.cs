using MeterReadings.Logic.Interface;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MeterReadings.Logic.Providers
{
    public class ConnectionProvider : IConnectionProvider
    {
        public static ConnectionProvider Instance
        {
            get
            {
                return new ConnectionProvider();
            }
        }

        public IDbConnection GetConnection()
        {
            return GetConnection(_defaultConnectionName);
        }

        public IDbConnection GetConnection(string connectionName)
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings[connectionName].ConnectionString);
        }

        private static readonly string _defaultConnectionName = "AppConnectionString";
    }
}
