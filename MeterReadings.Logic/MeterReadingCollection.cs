using Crudinski.Entity.Logic.Context;
using Crudinski.Entity.Logic.Items;
using Crudinski.Entity.Logic.Lists;
using MeterReadings.Logic.Interface;
using MeterReadings.Logic.Providers;
using MeterReadings.Model;
using System.Data;

namespace MeterReadings.Logic
{
    public class MeterReadingCollection<T> : DataItemList<T>
        where T : DataItem, new()
    {
        public override DataHouse DataHouse { get; } = new MeterReadingsDataHouse();

        public static void ReadMeterReadingCollection(MeterReadingCollection<T> collection, IConnectionProvider connectionProvider)
        {
            using (IDbConnection connection = connectionProvider.GetConnection())
            {
                connection.Open();
                collection.Read(connection, null, ParserProvider.Instance);
                connection.Close();
            }
        }
    }
}
