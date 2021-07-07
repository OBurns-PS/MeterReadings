using Crudinski.Data.Model.Interface;
using Crudinski.Data.QueryParser.Model;
using Crudinski.Data.Tsql.Parser;
using System.Data;

namespace MeterReadings.Logic.Providers
{
    public class ParserProvider : IParserProvider
    {
        public static ParserProvider Instance
        {
            get
            {
                return new ParserProvider();
            }
        }

        public ParserBase GetParser(IDbConnection connection)
        {
            return new TsqlParser();
        }
    }
}
