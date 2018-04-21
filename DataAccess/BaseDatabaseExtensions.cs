namespace DataAccess
{
    public static class BaseDatabaseExtensions
    {
        public static IFetchRequest From(this BaseDatabase db, string tableName)
        {
            return db.CreateFetchRequest(tableName);
        }
    }
}