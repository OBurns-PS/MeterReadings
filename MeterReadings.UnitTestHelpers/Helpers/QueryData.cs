using System.Collections.Generic;
using System.Data;

namespace MeterReadings.UnitTestHelpers.Helpers
{
    public class QueryData
    {
        public string CommandText { get; set; }
        public List<IDbDataParameter> CommandParameters { get; }

        public QueryData(string commandText, List<IDbDataParameter> commandParameters)
        {
            CommandText = commandText;
            CommandParameters = commandParameters;
        }
    }
}
