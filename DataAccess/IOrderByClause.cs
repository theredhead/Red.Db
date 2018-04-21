namespace DataAccess
{
    public interface IOrderByClause
    {
        string ColumnName { get; }
        SortDirection Direction { get; }
    }
}