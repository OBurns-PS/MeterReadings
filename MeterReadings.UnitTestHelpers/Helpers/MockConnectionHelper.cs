using Moq;
using System.Data;

namespace MeterReadings.UnitTestHelpers.Helpers
{
    public class MockConnectionHelper
    {
        public Mock<IDbConnection> Connection { get; set; }

        public Mock<IDbTransaction> Transaction { get; set; }

        private MockConnectionHelper() { }

        public static MockConnectionHelper CreateMockConnection()
        {
            return CreateMockConnection(
                new Mock<IDbConnection>(), new Mock<IDbTransaction>());
        }

        private static MockConnectionHelper CreateMockConnection(Mock<IDbConnection> mockConnection,
            Mock<IDbTransaction> mockTransaction)
        {
            MockConnectionHelper result = new MockConnectionHelper();
            mockConnection.Setup(m => m.BeginTransaction()).Returns(mockTransaction.Object);
            result.Connection = mockConnection;
            result.Transaction = mockTransaction;
            return result;
        }

        public void SetEmptyCommands()
        {
            Mock<IDbCommand> emptyCommand = new Mock<IDbCommand>();
            Mock<IDataParameterCollection> mockParameters = new Mock<IDataParameterCollection>();
            emptyCommand.SetupGet(m => m.Parameters).Returns(mockParameters.Object);
            emptyCommand.Setup(m => m.ExecuteNonQuery()).Returns(1);
            Connection.Setup(m => m.CreateCommand()).Returns(emptyCommand.Object);
        }
    }
}
