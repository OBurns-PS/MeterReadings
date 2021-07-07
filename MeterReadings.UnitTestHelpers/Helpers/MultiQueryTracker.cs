using Moq;
using System.Collections.Generic;
using System.Data;

namespace MeterReadings.UnitTestHelpers.Helpers
{
    public class MultiQueryTracker
    {
        public List<QueryData> QueryData { get; } = new List<QueryData>();
        public Mock<IDbConnection> MockConnection { get; private set; }
        public Mock<IDbTransaction> MockTransaction { get; }

        public MultiQueryTracker()
        {
            MockTransaction = new Mock<IDbTransaction>();
            SetupMockConnection();
        }

        private void SetupMockConnection()
        {
            Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(m => m.CreateCommand()).Returns(() => new MultiQueryItem(this).Command);
            mockConnection.Setup(m => m.BeginTransaction()).Returns(MockTransaction.Object);
            MockConnection = mockConnection;
        }
    }
}
