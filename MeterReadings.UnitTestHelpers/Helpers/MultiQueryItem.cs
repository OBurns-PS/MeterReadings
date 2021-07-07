using Moq;
using System;
using System.Collections.Generic;
using System.Data;

namespace MeterReadings.UnitTestHelpers.Helpers
{
    internal class MultiQueryItem
    {
        public string CommandText { get; private set; }
        public List<IDbDataParameter> CommandParameters { get; } = new List<IDbDataParameter>();
        public IDbCommand Command { get; private set; }

        public MultiQueryItem(MultiQueryTracker multiQueryTracker)
        {
            _multiQueryTracker = multiQueryTracker;
            CreateAndSetupCommand();
        }

        private void CreateAndSetupCommand()
        {
            Mock<IDbCommand> result = new Mock<IDbCommand>();
            result.Setup(m => m.CreateParameter()).Returns(() => CreateParameter());
            Mock<IDataParameterCollection> mockParameterCollection = new Mock<IDataParameterCollection>();

            result.SetupSet<string>(m => m.CommandText = It.IsAny<string>())
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

            result.Setup(m => m.ExecuteReader()).Callback(() =>
                _multiQueryTracker.QueryData.Add(new QueryData(CommandText, CommandParameters)));

            result.Setup(m => m.ExecuteNonQuery()).Callback(() =>
                _multiQueryTracker.QueryData.Add(new QueryData(CommandText, CommandParameters)));

            result.Setup(m => m.ExecuteScalar())
                .Returns(() => _scalarIDValues++)
                .Callback(() =>
                _multiQueryTracker.QueryData.Add(new QueryData(CommandText, CommandParameters)));

            result.SetupGet(m => m.Parameters).Returns(mockParameterCollection.Object);
            Command = result.Object;
        }

        private static IDbDataParameter CreateParameter()
        {
            Mock<IDbDataParameter> result = new Mock<IDbDataParameter>();
            result.SetupProperty(m => m.Value);
            result.SetupProperty(m => m.ParameterName);
            result.SetupProperty(m => m.DbType);
            return result.Object;
        }

        private static int _scalarIDValues = 1;
        private readonly MultiQueryTracker _multiQueryTracker;
    }
}
