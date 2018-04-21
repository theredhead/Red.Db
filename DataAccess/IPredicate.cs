using System.Collections.Generic;

namespace DataAccess
{
    public interface IPredicate
    {
        string Text { get; }
        IEnumerable<object> Arguments { get; }
    }
}