using System.Collections.Generic;

namespace DataAccess
{
    public interface IFetchRequest
    {
        string TableName { get; set; }
        IEnumerable<string> ColumnNames { get; set; }

        IEnumerable<IPredicate> Predicates { get; }
        IEnumerable<IOrderByClause> OrderClauses { get; }

        IPredicate AddPredicate(IPredicate predicate);
        IPredicate CreatePredicate(string text, params object[] arguments);

        IOrderByClause AddOrderClauser(IOrderByClause createOrderByClause);
        IOrderByClause CreateOrderByClause(string columnName, SortDirection direction);
    }
}