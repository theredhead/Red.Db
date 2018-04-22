using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DataAccess
{
    public abstract class BaseDatabase : IDatabase
    {
        protected virtual bool UsesPositionalParameters => false;

        public abstract string QuoteObjectName(string tableName);

        public abstract IDbConnection CreateConnection();

        public virtual string ParameterName(int index)
        {
            return UsesPositionalParameters
                ? "?"
                : ParameterName($"p_{index}");
        }

        public virtual IDbCommand CreateCommand(IFetchRequest request)
        {
            var argumentCount = 0;
            string SqlDir(SortDirection direction)
            {
                return direction == SortDirection.Ascending ? "ASC" : "DESC";
            }

            var builder = new StringBuilder();

            builder.Append("SELECT ");

            if (request.ColumnNames?.Any() ?? false)
            {
                var expandedColumnNames =
                    from columnName in request.ColumnNames
                    select QuoteObjectName(columnName);

                builder.AppendJoin(", ", expandedColumnNames);
            }
            else
            {
                builder.Append("*");
            }

            builder.Append(" FROM ");
            builder.Append(QuoteObjectName(request.TableName));

            if (request.Predicates.Any())
            {
                var expandedPredicateTexts =
                    from predicate in request.Predicates
                    select predicate.Text;

                builder.Append(" WHERE ");
                builder.AppendJoin(" AND ", expandedPredicateTexts);
            }

            if (request.OrderClauses.Any())
            {
                var expandedOrderByClauses =
                    from orderBy in request.OrderClauses
                    select $"{QuoteObjectName(orderBy.ColumnName)} {SqlDir(orderBy.Direction)}";

                builder.Append(" ORDER BY ");
                builder.AppendJoin(", ", expandedOrderByClauses);
            }

            var commandText = builder.ToString();
            var arguments = request.Predicates.SelectMany(p => p.Arguments).ToArray();

            return CreateCommand(commandText, arguments);
        }

        public virtual IDbCommand CreateCommand(string commandText, params object[] arguments)
        {
            var command = CreateConnection().CreateCommand();

            command.CommandText = ExpandCommandText(commandText);

            for (var ix = 0; ix < arguments.Length; ix++)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = ParameterName(ix);
                parameter.Value = arguments[ix];
                command.Parameters.Add(parameter);
            }

            return command;
        }

        public virtual int ExecuteNonQuery(string commandText, params object[] arguments)
        {
            int result = 0;
            using (var command = CreateCommand(commandText, arguments))
            {
                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();

                result = command.ExecuteNonQuery(); 
                command.Connection.Close();
            }

            return result;
        }
        public virtual IEnumerable<IDataRecord> Execute(string commandText, params object[] arguments)
        {
            using (var command = CreateCommand(commandText, arguments))
            {
                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read()) yield return reader;
                }
                command.Connection.Close();
            }
        }

        public virtual DataTable ExecuteDataTable(string commandText, params object[] arguments)
        {
            var table = new DataTable();

            using (var command = CreateCommand(commandText, arguments))
            {
                using (var reader = command.ExecuteReader())
                {
                    table.Load(reader);
                }
            }

            return table;
        }

        public virtual DataSet ExecuteDataSet(string commandText, params object[] arguments)
        {
            var set = new DataSet();

            using (var command = CreateCommand(commandText, arguments))
            {
                using (var reader = command.ExecuteReader())
                {
                    set.Load(reader, LoadOption.Upsert, new string[] { });
                }
            }

            return set;
        }

        public virtual T ExecuteScalar<T>(string commandText, params object[] arguments)
        {
            return CreateCommand(commandText, arguments).ExecuteScalar<T>();
        }

        public virtual IEnumerable<IDataRecord> Fetch(IFetchRequest request)
        {
            using (var command = CreateCommand(request))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read()) yield return reader;
                }
            }
        }
        
        public virtual IEnumerable<T> Fetch<T>(IFetchRequest request) where T : IDataRecordLoadable, new() 
        {
            using (var command = CreateCommand(request))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read()) yield return MakeInstanceWithRecord<T>(reader);
                }
            }
        }

        protected virtual T MakeInstanceWithRecord<T>(IDataRecord record) where T : IDataRecordLoadable, new()
        {
            var instance = new  T();
            instance.Load(record);
            return instance;
        }

        protected virtual string ParameterName(string name)
        {
            return "@" + name;
        }

        public IFetchRequest CreateFetchRequest() => new FetchRequest();
        
        public IFetchRequest CreateFetchRequest(string tableName)
        {
            var request = CreateFetchRequest();
            request.TableName = tableName;
            return request;
        }
        
        protected virtual string ExpandCommandText(string commandText)
        {
            int argumentCount = 0;

            var builder = new StringBuilder();
            var length = commandText.Length;

            for (var index = 0; index < length; index++)
            {
                var currentCharacter = commandText[index];

                if (currentCharacter == '?')
                {
                    if (index < length - 1)
                    {
                        var nextCharacter = commandText[index + 1];
                        if (nextCharacter == '?')
                        {
                            builder.Append('?');
                            index++;
                            continue;
                        }
                    }

                    builder.Append(ParameterName(argumentCount));
                    argumentCount++;
                }
                else
                {
                    builder.Append(currentCharacter);
                }
            }

            return builder.ToString();
        }
    }
}