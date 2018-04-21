using System;
using System.Data.SQLite;    

namespace DataAccess.Sqlite
{
    public class SqliteDatabase : Database<SQLiteConnection>
    {
        public SqliteDatabase(string connectionString) : base(connectionString)
        {
        }

        protected override string QuoteColumnName(string columnName)
        {
            return $"\"{columnName}\"";
        }

        protected override string QuoteTableName(string tableName)
        {
            return $"\"{tableName}\"";
        }
    }}