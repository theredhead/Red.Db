namespace DataAccess
{
    public static class IFetchRequestExtras
    {
        public static IFetchRequest Where(this IFetchRequest request, string text, params object[] arguments)
        {
            request.AddPredicate(request.CreatePredicate(text, arguments));
            return request;
        }

        public static IFetchRequest OrderBy(this IFetchRequest request, string columnName,
            SortDirection direction = SortDirection.Ascending)
        {
            request.AddOrderClauser(request.CreateOrderByClause(columnName, direction));
            return request;
        }

//        public static DataTable SelectTable(this IFetchRequest request)
//        {
//            var table = new DataTable();
//            using (var reader = request.Select())
//            {
//                table.Load(reader);
//            }
//            return table;
//        }
    }
}