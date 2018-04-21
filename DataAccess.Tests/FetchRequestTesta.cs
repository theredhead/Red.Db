using System.Linq;
using DataAccess.Sqlite;
using Xunit;

namespace DataAccess.Tests
{
    public class FetchRequestTests
    {
        private static readonly string SqliteFilePath =
            @"../../../Data/Database.sqlite3";

        private static readonly string ConnectionString =
            $"Data Source={SqliteFilePath};Version=3;";


        [Fact]
        public void CanFetchPeopleOrderedByBirthdate()
        {
            var db = new SqliteDatabase(ConnectionString);

            var request = db
                .From("Person")
                .OrderBy("Birthdate", SortDirection.Descending);

            var numberOfeople = db.Fetch(request).Count();

            Assert.Equal(11, numberOfeople); // Known number of records
        }
    }
}