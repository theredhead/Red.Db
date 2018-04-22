using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess.Sqlite;
using Xunit;

namespace DataAccess.Tests
{
    public class FetchRequestTests
    {
        private enum Gender
        {
            Male = 1,
            Female = 2,
            Other = 3,
            Unknown = 4
        }

        private class Person : Record
        {
            public long Id 
                => Convert.ToInt64(this[nameof(Id)]);
            
            public string Name 
                => Convert.ToString(this[nameof(Name)]);
            
            public string Surname 
                => Convert.ToString(this[nameof(Surname)]);

            public DateTime? Birthdate
                => TryGet<DateTime?>(nameof(Birthdate), null);

            public DateTime? Died
                => TryGet<DateTime?>(nameof(Died), null);

            public Gender Gender 
                => TryGet<Gender>(nameof(Gender), Gender.Other);
        }
        
        private const string ConnectionString = "Data Source=Data/Database.sqlite3;Version=3;";


        [Fact]
        public void CanFetchPeopleOrderedByBirthdate()
        {
            var db = new SqliteDatabase(ConnectionString);

            var request = db
                .From("Person")
                .OrderBy("Birthdate", SortDirection.Descending);

            var numberOfeople = db.Fetch(request).Count();

            Assert.True(numberOfeople > 0);
        }

        [Theory]
        [InlineData("Steve", "Jobs", "1955-02-24")]
        [InlineData("Albert", "Einstein", "1879-03-14")]
        [InlineData("Rosa", "Parks", "1913-02-04")]
        [InlineData("Amelia", "Earhart", "1897-07-02")]
        public void CanFindPeopleByMultipleColumns(string firstname, string lastname, DateTime birthdate)
        {            
            var db = new SqliteDatabase(ConnectionString);

            var request = db
                .From("Person")
                .Where("Name = ?", firstname)
                .Where("Surname = ?", lastname)
                // use of Date() function in comparison is an unfortunate requirement for two dates to match in Sqlite
                .Where("Date(Birthdate) = Date(?)", birthdate)
                .OrderBy("Birthdate", SortDirection.Descending);

            var people = db.Fetch<Person>(request).ToList();
            
            var found = people.Any();
            foreach(var person in people)
            {
                Assert.Equal(firstname, person.Name);                
                Assert.Equal(lastname, person.Surname);
                found = found && people.Count() == 1;
            }
            Assert.True(found);
        }

        [Fact]
        public void InsertWorks()
        {
            var db = new SqliteDatabase(ConnectionString);

            int CountPeople()
                => db.ExecuteScalar<int>("SELECT COUNT(1) FROM Person");

            var before = CountPeople();
            
            db.Insert("Person", new Dictionary<string, object>
            {
                {"Name", "Ada"},
                {"Surname", "Lovelace"},
                {"Gender", Gender.Female}
            });

            var after = CountPeople();
            
            Assert.Equal(before + 1, after);
        }

        [Fact]
        public void UpdateWorks()
        {
            var db = new SqliteDatabase(ConnectionString);

            int CountPeopleByGender(Gender gender)
                => db.ExecuteScalar<int>("SELECT COUNT(1) FROM Person WHERE Gender = ?", gender);

            var before = CountPeopleByGender(Gender.Female);
            
            var request = db
                .From("Person")
                .Where("Name IN(?, ?, ?)", "Ada", "Amelia", "Rosa");

            var women = db.Fetch<Person>(request).ToArray();

            foreach (var person in women)
            {
                db.Update("Person", person.Id, new Dictionary<string, object>
                {
                    {"Gender", Gender.Female}
                });
            }
            
            var after = CountPeopleByGender(Gender.Female);

            Assert.True(after > before);
        }
    }
}