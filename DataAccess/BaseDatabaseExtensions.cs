using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess
{
    public static class IDatabaseExtensions
    {
        public static IFetchRequest From(this IDatabase db, string tableName)
        {
            return db.CreateFetchRequest(tableName);
        }
        
        
        
        
        
        public static void Insert(this IDatabase db, string tableName, Dictionary<string, object> columnsAndValues)
        {
            var columnNames = string.Join(", ",
                from columnName in columnsAndValues.Keys
                select db.QuoteObjectName(columnName)
            );
            var quotedTableName = db.QuoteObjectName(tableName);

            var placeholders = string.Join(", ", Enumerable.Repeat("?", columnsAndValues.Count));
            
            var insertCommand = $"INSERT INTO {quotedTableName} ({columnNames}) VALUES ({placeholders})";
            
            db.ExecuteNonQuery(insertCommand, columnsAndValues.Values.ToArray());
        }
    
        public static void Update(this IDatabase db, string tableName, long id, Dictionary<string, object> columnsAndValues, string idColumnName="Id") 
        {
            var setters = string.Join(", ",
                from pair in columnsAndValues
                select $"{db.QuoteObjectName(pair.Key)} = ?"
            );
            var quotedTableName = db.QuoteObjectName(tableName);
            var quotedIdColumnName = db.QuoteObjectName(idColumnName);
        
            var insertCommand = $"UPDATE {quotedTableName} SET {setters} WHERE {quotedIdColumnName} = ?";
            var arguments = new List<object>(columnsAndValues.Values) {id};

            db.ExecuteNonQuery(insertCommand, arguments.ToArray());
        }
    }
}