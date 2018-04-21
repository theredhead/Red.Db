using System;
using System.Collections.Generic;
using System.Data;

namespace DataAccess
{
    public abstract class Database<T> : BaseDatabase where T : IDbConnection
    {
        private readonly List<IDbConnection> _connections = new List<IDbConnection>();
        private readonly string _connectionString;

        protected Database(string connectionString)
        {
            _connectionString = connectionString;
        }

        public override IDbConnection CreateConnection()
        {
            var connection = (IDbConnection) Activator.CreateInstance(typeof(T), _connectionString);
            connection.Open();
            _connections.Add(connection);
            return connection;
        }
    }
}