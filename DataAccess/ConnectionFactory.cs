using System;
using System.Data;

namespace DataAccess
{
    public class ConnectionFactory<T> : IConnectionFactory where T : IDbConnection
    {
        public ConnectionFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; set; }

        public IDbConnection CreateConnection()
        {
            return (T) Activator.CreateInstance(typeof(T), ConnectionString);
        }
    }
}