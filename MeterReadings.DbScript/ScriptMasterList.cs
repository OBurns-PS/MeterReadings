using Crudinski.Tools.DbScript.Model.Interface;
using MeterReadings.DbScript.Scripts;
using System.Collections.Generic;

namespace MeterReadings.DbScript
{
    public static class ScriptMasterList
    {
        public static List<IDbScript> MasterList { get; } = new List<IDbScript>()
        {
            new InitialiseTables(),
            new SeedAccountData()
        };
    }
}
