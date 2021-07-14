using MeterReadings.Model.Objects;
using System.Collections.Generic;

namespace MeterReadings.Logic.Interface
{
    public interface IAccountCollection
    {
        List<Account> GetAccounts(IEnumerable<int> accountIds);
        void SubmitAccountReadings();
    }
}
