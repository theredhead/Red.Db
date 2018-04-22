using System.Data.SqlClient;

namespace DataAccess.SqlServer
{
    public class SqlServerDatabase : Database<SqlConnection>
    {
        public SqlServerDatabase(string connectionString) : base(connectionString)
        {
        }

        public override string QuoteObjectName(string objectName) => $"`{objectName}`";
    }
}