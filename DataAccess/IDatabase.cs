using System.Collections.Generic;
using System.Data;
using JetBrains.Annotations;

namespace DataAccess
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public interface IDatabase
    {
        IDbConnection CreateConnection();

        string QuoteObjectName(string objectName);
        string ParameterName(int index);

        // Command creation
        IDbCommand CreateCommand(IFetchRequest request);
        IDbCommand CreateCommand(string commandText, params object[] arguments);
        
        // Command execution
        IEnumerable<IDataRecord> Execute(string commandText, params object[] arguments);
        int ExecuteNonQuery(string commandText, params object[] arguments);
        DataTable ExecuteDataTable(string commandText, params object[] arguments);
        DataSet ExecuteDataSet(string commandText, params object[] arguments);
        T ExecuteScalar<T>(string commandText, params object[] arguments);

        // Creating FetchRequests
        IFetchRequest CreateFetchRequest();
        IFetchRequest CreateFetchRequest(string tableName);
        
        // Fething data using FetchRequests
        IEnumerable<IDataRecord> Fetch(IFetchRequest request);
        IEnumerable<T> Fetch<T>(IFetchRequest request) where T : IDataRecordLoadable, new();
    }
}