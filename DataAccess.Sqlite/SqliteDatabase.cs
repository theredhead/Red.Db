using System.Data.SQLite;

namespace DataAccess.Sqlite
{
    public class SqliteDatabase : Database<SQLiteConnection>
    {
        public SqliteDatabase(string connectionString) : base(connectionString)
        {
        }

        public override string QuoteObjectName(string objectName) => $"\"{objectName}\"";
    }
}