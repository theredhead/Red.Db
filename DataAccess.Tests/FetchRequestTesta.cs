using System;
using System.Collections.Generic;
using System.Data;
using DataAccess.MySql;
using DataAccess.Sqlite;
using MySql.Data.MySqlClient;
using Xunit;

namespace DataAccess.Tests
{

    public class FetchRequestTests
    {
        private static readonly string SqliteFilePath =
            @"/Users/kris/Projects/Red.Db/DataAccess.Tests/Data/Database.sqlite3";

        private static readonly string ConnectionString = $"Data Source={SqliteFilePath};Version=3;";
        
        [Fact]
        public void CanFetchPeopleOrderedByBirthdate()
        {
            var db = new SqliteDatabase(ConnectionString);

            var request = db
                .From("Person")
                .OrderBy("Birthdate", SortDirection.Descending);

            var counted = 0L;
            
            foreach (var record in db.Fetch(request))
                counted++;

            Assert.Equal(11, counted); // Known number of records
        }
    }    
}