using System.Collections.Generic;

namespace DataAccess
{
    public class FetchRequest : IFetchRequest
    {
        private readonly List<IOrderByClause> _orderClauses = new List<IOrderByClause>();

        private readonly List<IPredicate> _predicates = new List<IPredicate>();

        public IEnumerable<string> ColumnNames { get; set; } = new string[] { };

        public string TableName { get; set; }
        public IEnumerable<IPredicate> Predicates => _predicates;

        public IEnumerable<IOrderByClause> OrderClauses => _orderClauses;

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
            return new OrderByClause
            {
                ColumnName = columnName,
                Direction = direction
            };
        }

        public FetchRequest Where(string text, params object[] arguments)
        {
            AddPredicate(CreatePredicate(text, arguments));
            return this;
        }

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
    }
}