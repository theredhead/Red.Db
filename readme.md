Red.Db
---------

This set of libraries provides simplified database access. currently supporting:

- SQLite
- MySql
- SqlServer

other ADO.NET compatible engines are super easy to implement.

FetchRequest is intended to serve as a friendly means of low-level record selection with a simple API:

```
SqliteDatabase db = new SqliteDatabase(SQLITE_CONNECTIONSTRING);

var request = db
.From("Person")
.Where("Age BETWEEN ? AND ?", 30, 35)
.OrderBy("Age", SortDirection.Descending);

foreach (var record in db.Fetch(request))
	DoSomethingWithPerson(record);

```



