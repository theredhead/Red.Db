using MySql.Data.MySqlClient;

namespace DataAccess.MySql
{
    public class MySqlDatabase : Database<MySqlConnection>
    {
        public MySqlDatabase(string connectionString) : base(connectionString)
        {
        }

        public override string QuoteObjectName(string objectName) => $"`{objectName}`";
    }
}