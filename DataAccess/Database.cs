using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DataAccess
{
    public abstract class Database<T> : BaseDatabase where T : IDbConnection
    {
        private readonly string _connectionString;
        private readonly List<IDbConnection> _connections = new List<IDbConnection>(); 

        public override IDbConnection CreateConnection()
        {
            var connection = (IDbConnection) Activator.CreateInstance(typeof(T), new object[] {_connectionString});
            connection.Open();
            _connections.Add(connection);
            return connection;
        }

        protected Database(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}