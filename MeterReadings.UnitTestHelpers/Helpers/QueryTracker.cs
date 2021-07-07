using Moq;
using System;
using System.Collections.Generic;
using System.Data;

namespace MeterReadings.UnitTestHelpers.Helpers
{
    public sealed class QueryTracker
    {
        public string CommandText { get; private set; } = string.Empty;
        public List<IDbDataParameter> CommandParameters { get; } = new List<IDbDataParameter>();
        public Mock<IDbConnection> MockConnection { get; private set; }
        public Mock<IDbCommand> MockCommand { get; private set; }

        public QueryTracker()
        {
            MockConnection = CreateMockConnection();
        }

        private static IDbDataParameter CreateParameter()
        {
            Mock<IDbDataParameter> result = new Mock<IDbDataParameter>();
            result.SetupProperty(m => m.Value);
            result.SetupProperty(m => m.ParameterName);
            result.SetupProperty(m => m.DbType);
            return result.Object;
        }

        private Mock<IDbConnection> CreateMockConnection()
        {
            Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            MockCommand = new Mock<IDbCommand>();
            Mock<IDataParameterCollection> mockParameterCollection = new Mock<IDataParameterCollection>();

            MockCommand.Setup(m => m.CreateParameter()).Returns(() => CreateParameter());

            MockCommand.SetupSet<string>(m => m.CommandText = It.IsAny<string>())
                .Callback(
                (s) =>
                {
                    CommandText = s;
                });

            Action<object> addParameterCallback = new Action<object>(
                (p) =>
                {
                    if (p is IDbDataParameter param)
                    {
                        CommandParameters.Add(param);
                    }
                }
                );

            mockParameterCollection.Setup(m => m.Add(It.IsAny<IDbDataParameter>())).Callback(
                (object param) => addParameterCallback(param));
            mockParameterCollection.Setup(m => m.Count).Returns(() => CommandParameters.Count);

            MockCommand.SetupGet(m => m.Parameters).Returns(mockParameterCollection.Object);
            mockConnection.Setup(m => m.CreateCommand()).Returns(MockCommand.Object);

            return mockConnection;
        }
    }
}
