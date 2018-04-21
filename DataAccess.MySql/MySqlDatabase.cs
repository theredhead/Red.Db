using MySql.Data.MySqlClient;

namespace DataAccess.MySql
{
    public class MySqlDatabase : Database<MySqlConnection>
    {
        public MySqlDatabase(string connectionString) : base(connectionString)
        {
        }

        protected override string QuoteColumnName(string columnName)
        {
            return $"`{columnName}`";
        }

        protected override string QuoteTableName(string tableName)
        {
            return $"`{tableName}`";
        }
    }
}