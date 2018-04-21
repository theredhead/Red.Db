using System.Collections.Generic;
using System.Data;

namespace DataAccess
{
    public class FetchRequest : IFetchRequest
    {
        private class Predicate : IPredicate
        {
            public string Text { get; internal set; }
            public IEnumerable<object> Arguments { get; internal set; }
        }

        private class OrderByClause : IOrderByClause
        {
            public string ColumnName { get; internal set; }
            public SortDirection Direction { get; internal set; }
        }

        public IEnumerable<string> ColumnNames { get; set; } = new string[] { };

        public string TableName { get; set; }
        
        private readonly List<IPredicate> _predicates = new List<IPredicate>();
        private readonly List<IOrderByClause> _orderClauses = new List<IOrderByClause>();
        public IEnumerable<IPredicate> Predicates => _predicates;

        public IEnumerable<IOrderByClause> OrderClauses => _orderClauses;

        public FetchRequest Where(string text, params object[] arguments)
        {
            AddPredicate(CreatePredicate(text, arguments));
            return this;
        }

        public IPredicate CreatePredicate(string text, params object[] arguments)
        {
            var predicate = new Predicate
            {
                Text = text,
                Arguments = arguments
            };
            return predicate;
        }

        public IPredicate AddPredicate(IPredicate predicate)
        {
            _predicates.Add(predicate);
            return predicate;
        }

        public IOrderByClause AddOrderClauser(IOrderByClause orderByClause)
        {
            _orderClauses.Add(orderByClause);
            return orderByClause;
        }

        public IOrderByClause CreateOrderByClause(string columnName, SortDirection direction)
        {
            return new OrderByClause()
            {
                ColumnName = columnName,
                Direction = direction
            };
        }

//        public IDataReader Select()
//        {
//            return Select(new string[] {});
//        }
//
//        public IDataReader Select(params string[] columnNames)
//        {
//            ColumnNames = columnNames;
//            return Database.CreateCommand(this).ExecuteReader();
//        }
    }
}