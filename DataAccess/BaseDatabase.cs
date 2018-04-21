using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DataAccess
{
    public delegate void OnRecordDelegate(IDataRecord record);
    
    public abstract class BaseDatabase : IDatabase
    {
        protected virtual bool UsesPositionalParameters => false;

        public abstract IDbConnection CreateConnection();

        protected virtual string ParameterName(string name)
        {
            return "@" + name;
        }

        public virtual string ParameterName(int index)
        {
            return UsesPositionalParameters 
                ? "?" 
                : ParameterName($"p_{index}");
        }

        protected abstract string QuoteColumnName(string columnName);
        protected abstract string QuoteTableName(string tableName);

        public IFetchRequest CreateFetchRequest(string tableName)
        {
            var request = new FetchRequest
            { 
                TableName = tableName
            };
            return request;
        }

        public virtual IDbCommand CreateCommand(IFetchRequest request)
        {
            string SqlDir(SortDirection direction) 
                => direction == SortDirection.Ascending ? "ASC" : "DESC";
            
            var builder = new StringBuilder();

            builder.Append("SELECT ");

            if (request.ColumnNames?.Any() ?? false)
            {
                var expandedColumnNames =
                    from columnName in request.ColumnNames
                    select QuoteColumnName(columnName);

                builder.AppendJoin(", ", expandedColumnNames);
            }
            else
                builder.Append("*");

            builder.Append(" FROM ");
            builder.Append(QuoteTableName(request.TableName));

            if (request.Predicates.Any())
            {
                var expandedPredicateTexts =
                    from predicate in request.Predicates
                    select ExpandCommandText(predicate.Text);

                builder.Append(" WHERE ");
                builder.AppendJoin(" AND ", expandedPredicateTexts);
            }

            if (request.OrderClauses.Any())
            {
                var expandedOrderByClauses =
                    from orderBy in request.OrderClauses
                    select $"{QuoteColumnName(orderBy.ColumnName)} {SqlDir(orderBy.Direction)}";

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

        protected virtual string ExpandCommandText(string commandText)
        {
            var builder = new StringBuilder();
            var length = commandText.Length;
            var argumentCount = 0;

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
                    builder.Append(ParameterName(argumentCount++));
                }
                else
                    builder.Append(currentCharacter);
            }

            return builder.ToString();
        }

        public virtual IEnumerable<IDataRecord> Execute(string commandText, params object[] arguments)
        {
            using (var command = CreateCommand(commandText, arguments))
            {
                command.Connection.Open();
                using(var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
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
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }
    }
}