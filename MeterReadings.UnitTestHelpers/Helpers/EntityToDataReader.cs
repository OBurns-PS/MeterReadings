using BRepo.ObjectFunctions;
using Crudinski.Data.Attributes;
using Crudinski.DataTypes.Interface;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace MeterReadings.UnitTestHelpers.Helpers
{
    public class EntityToDataReader
    {
        public Mock<IDbConnection> Connection { get; }
        public Mock<IDbTransaction> Transaction { get; }
        public Mock<IDbCommand> Command { get; }

        public string CurrentQueryCommandText
        {
            get
            {
                return _currentQueryCommandText;
            }
        }

        public IDataReader Reader { get; private set; }

        public EntityToDataReader()
            : this(false)
        { }

        private IDbDataParameter CreateParameter()
        {
            Mock<IDbDataParameter> result = new Mock<IDbDataParameter>();
            result.SetupProperty(m => m.ParameterName);
            result.SetupProperty(m => m.DbType);
            result.SetupProperty(m => m.Value);
            return result.Object;
        }

        public EntityToDataReader(bool setEmptyDbCommands)
        {
            MockConnectionHelper connectionHelper 
                = MockConnectionHelper.CreateMockConnection();

            if (setEmptyDbCommands)
            {
                connectionHelper.SetEmptyCommands();
            }

            Connection = connectionHelper.Connection;
            Transaction = connectionHelper.Transaction;
            _conditionalResponses = new Dictionary<Func<string, bool>, IEnumerable<IDataItem>>();
            _queryParameters = new List<IDbDataParameter>();
            _conditionalQueryResponses = new Dictionary<Func<IDbDataParameter, bool>, IEnumerable<IDataItem>>();

            Mock<IDbCommand> mockCommand = new Mock<IDbCommand>();

            Mock<IDataParameterCollection> mockCommandParameters
                = new Mock<IDataParameterCollection>();

            mockCommandParameters.Setup(m => m.Add(It.IsAny<object>())).Callback<object>((o) => AddQueryParameter(o));

            mockCommand.SetupGet(m => m.Parameters).Returns(mockCommandParameters.Object);

            mockCommand.SetupSet(m => m.CommandText = It.IsAny<string>()).Callback<string>(
                (s) => SetCurrentQueryCommandText(s));

            mockCommand.Setup(m => m.CreateParameter()).Returns(() => CreateParameter());
            mockCommand.Setup(m => m.ExecuteReader()).Returns(() => GetDataFromCommand());
            Connection.Setup(m => m.CreateCommand()).Returns(mockCommand.Object);
            Command = mockCommand;
        }

        public void SetupMockResponse<T>(Func<string, bool> commandTextQuery, IEnumerable<T> objects)
            where T : IDataItem
        {
            _conditionalResponses[commandTextQuery] = objects.Cast<IDataItem>();
        }

        public void SetupMockResponse<T>(Func<IDbDataParameter, bool> functionEval, IEnumerable<T> objects)
            where T : IDataItem
        {
            _conditionalQueryResponses[functionEval] = objects.Cast<IDataItem>();
        }

        private void SetCurrentQueryCommandText(string commandText)
        {
            _currentQueryCommandText = commandText;
            _queryParameters.Clear();
        }

        private void AddQueryParameter(object queryParameter)
        {
            if(queryParameter is IDbDataParameter dataParameter)
            {
                _queryParameters.Add(dataParameter);
            }
        }

        private IDataReader GetDataFromCommand()
        {
            foreach(KeyValuePair<Func<string, bool>, IEnumerable<IDataItem>> condition in _conditionalResponses)
            {
                if (condition.Key(_currentQueryCommandText))
                {
                    return ReaderFromObject(condition.Value);
                }
            }

            foreach(IDbDataParameter queryParameter in _queryParameters)
            {
                foreach(KeyValuePair<Func<IDbDataParameter, bool>, IEnumerable<IDataItem>> condition in _conditionalQueryResponses)
                {
                    if (condition.Key(queryParameter))
                    {
                        return ReaderFromObject(condition.Value);
                    }
                }
            }

            Mock<IDataReader> mockDefaultReader = new Mock<IDataReader>();
            mockDefaultReader.Setup(m => m.Read()).Returns(false);
            return mockDefaultReader.Object;
        }

        private IDataReader ReaderFromObject<T>(IEnumerable<T> objects)
            where T : IDataItem
        {
            Type listObjectType = objects.GetType().GenericTypeArguments[0];
            using (DataTable table = CreateTableFromEntity(listObjectType))
            {
                PopulateTableFromObjects(table, objects);
                return table.CreateDataReader();
            }
        }

        private static DataTable CreateTableFromEntity(Type itemType)
        {
            DataTable result = new DataTable();
            Dictionary<string, MappedFieldAttribute> mappedFields = GetMappedFields(itemType);

            IEnumerable<string> mappedReadFields = mappedFields
                .Select(x => x.Key);
            
            foreach (string mappedFieldName in mappedReadFields)
            {
                Type fieldType = itemType.GetProperty(mappedFieldName).PropertyType;
                DataColumn column = new DataColumn(mappedFieldName);
                if(Properties.IsPropertyNullable(fieldType, out Type nullableType))
                {
                    column.AllowDBNull = true;
                    column.DataType = nullableType;
                }
                else
                {
                    column.DataType = fieldType;
                }
                result.Columns.Add(column);
            }

            return result;
        }

        private static void PopulateTableFromObjects<T>(DataTable table, IEnumerable<T> items)
            where T : IDataItem
        {
            foreach (T item in items)
            {
                PopulateTableFromObject(table, item);
            }
        }

        private static void PopulateTableFromObject<T>(DataTable table, T item)
            where T : IDataItem
        {
            object[] rowValues = new object[table.Columns.Count];

            for (int i = 0; i < rowValues.Length; i++)
            {
                rowValues[i] = item.GetPropertyValue(table.Columns[i].ColumnName) ?? DBNull.Value;
            }
            table.Rows.Add(rowValues);
        }

        private static Dictionary<string, MappedFieldAttribute> GetMappedFields(
           Type itemType)
        {
            string classFullName = $"{itemType.Namespace}.{itemType.Name}";
            lock (_cachedMappedFields)
            {
                if (!_cachedMappedFields.TryGetValue(classFullName, out _))
                {
                    Dictionary<string, MappedFieldAttribute> newMappedFields
                        = new Dictionary<string, MappedFieldAttribute>();

                    IEnumerable<PropertyInfo> objectProperties =
                        itemType.GetProperties();

                    IEnumerable<IGrouping<string, MappedFieldAttribute>>
                        mappedGroupedProperties = objectProperties
                            .SelectMany(x => x.GetCustomAttributes<MappedFieldAttribute>(true))
                            .GroupBy(x => x.FieldName);

                    foreach (IGrouping<string, MappedFieldAttribute> propertyGroup in mappedGroupedProperties)
                    {
                        newMappedFields[propertyGroup.Key] = new MappedFieldAttribute(propertyGroup.Key)
                        {
                            FieldIndex = propertyGroup.Max(x => x.FieldIndex),
                            FieldLength = propertyGroup.Max(x => x.FieldLength)
                        };
                    }

                    _cachedMappedFields.Add(classFullName, newMappedFields);
                }
            }

            Dictionary<string, MappedFieldAttribute> result = new Dictionary<string, MappedFieldAttribute>();
            foreach (KeyValuePair<string, MappedFieldAttribute> attribute
                in _cachedMappedFields[classFullName])
            {
                result.Add(attribute.Key, CloneMappedField(attribute.Value));
            }
            return result;
        }

        private static MappedFieldAttribute CloneMappedField(MappedFieldAttribute mappedFieldAttribute)
        {
            return new MappedFieldAttribute(mappedFieldAttribute.FieldName)
            {
                FieldIndex = mappedFieldAttribute.FieldIndex,
                FieldLength = mappedFieldAttribute.FieldLength,
            };
        }

        private static readonly Dictionary<string, Dictionary<string, MappedFieldAttribute>> _cachedMappedFields
            = new Dictionary<string, Dictionary<string, MappedFieldAttribute>>();

        private readonly Dictionary<Func<string, bool>, IEnumerable<IDataItem>> _conditionalResponses;
        private readonly Dictionary<Func<IDbDataParameter, bool>, IEnumerable<IDataItem>> _conditionalQueryResponses;
        private readonly List<IDbDataParameter> _queryParameters;
        private string _currentQueryCommandText;
    }
}
