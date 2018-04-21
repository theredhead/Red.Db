using System.Data;

namespace DataAccess
{
    public interface IConnectionFactory
    {
        string ConnectionString { get; set; }
        
        IDbConnection CreateConnection();
    }
}