using System.Data;

namespace MeterReadings.Logic.Interface
{
    public interface IConnectionProvider
    {
        IDbConnection GetConnection();
        IDbConnection GetConnection(string connectionName);
    }
}
