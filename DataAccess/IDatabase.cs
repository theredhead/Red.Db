using System.Collections.Generic;
using System.Data;

namespace DataAccess
{
    public interface IDatabase
    {
        IDbConnection CreateConnection();
        string ParameterName(int index);
        
        IDbCommand CreateCommand(IFetchRequest request);
        IDbCommand CreateCommand(string commandText, params object[] arguments);
        IEnumerable<IDataRecord> Execute(string commandText, params object[] arguments);
        
        DataTable ExecuteDataTable(string commandText, params object[] arguments);
        DataSet ExecuteDataSet(string commandText, params object[] arguments);
        T ExecuteScalar<T>(string commandText, params object[] arguments);

        IEnumerable<IDataRecord> Fetch(IFetchRequest request);
    }
}