using System.Data.SqlClient;

namespace DataAccess.SqlServer
{
    public class SqlServerDatabase : Database<SqlConnection>
    {
        public SqlServerDatabase(string connectionString) : base(connectionString)
        {
        }

        protected override string QuoteColumnName(string columnName)
        {
            return $"[{columnName}]";
        }

        protected override string QuoteTableName(string tableName)
        {
            return $"[{tableName}]";
        }
    }
}