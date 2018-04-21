using System.Collections.Generic;
using System.Data;

namespace DataAccess
{
    public interface IFetchRequest
    {
        string TableName { get; }
        IEnumerable<string> ColumnNames { get; set; }

        IEnumerable<IPredicate> Predicates { get; }
        IEnumerable<IOrderByClause> OrderClauses { get; }

        IPredicate AddPredicate(IPredicate predicate);
        IPredicate CreatePredicate(string text, params object[] arguments);
                            
        IOrderByClause AddOrderClauser(IOrderByClause createOrderByClause);
        IOrderByClause CreateOrderByClause(string columnName, SortDirection direction);
    }
}