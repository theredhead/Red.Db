using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace DataAccess
{
    /// <summary>
    ///     Provides a series of low-level extension methods for working with IDbConnection and IDbCommand instances.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class ConnectionExtensions
    {
        // Creating single parameters.

        /// <summary>
        ///     Create a parameter with the given name, value and type
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IDbDataParameter CreateParameter([NotNull] this IDbCommand command,
            [NotNull] string name, object value, DbType type = DbType.AnsiString)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            parameter.DbType = type;
            return parameter;
        }

        /// <summary>
        ///     Create an output only parameter with the given name, value and type
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IDbDataParameter CreateOutputParameter([NotNull] this IDbCommand command,
            [NotNull] string name, DbType type = DbType.AnsiString)
        {
            var parameter = CreateParameter(command, name, DBNull.Value);
            parameter.Direction = ParameterDirection.Output;
            return parameter;
        }

        /// <summary>
        ///     Create a parameter for the return value with the given name and type
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IDbDataParameter CreateReturnValueParameter([NotNull] this IDbCommand command,
            [NotNull] string name, DbType type = DbType.AnsiString)
        {
            var parameter = CreateParameter(command, name, DBNull.Value);
            parameter.Direction = ParameterDirection.ReturnValue;
            return parameter;
        }

        public static IDbDataParameter CreateBiDirectionalParameter([NotNull] this IDbCommand command,
            [NotNull] string name, object value, DbType type = DbType.AnsiString)
        {
            var parameter = CreateParameter(command, name, value ?? DBNull.Value);
            parameter.Direction = ParameterDirection.InputOutput;
            return parameter;
        }

        // Creating and adding single parameters.

        /// <summary>
        ///     Add an input only parameter with the given name, value and type
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IDbDataParameter AddParameter([NotNull] this IDbCommand command,
            [NotNull] string name, object value, DbType type = DbType.AnsiString)
        {
            var parameter = CreateParameter(command, name, value, type);
            command.Parameters.Add(parameter);
            return parameter;
        }

        /// <summary>
        ///     Add an output only parameter with the given name and type
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IDbDataParameter AddOutputParameter([NotNull] this IDbCommand command,
            [NotNull] string name, DbType type = DbType.AnsiString)
        {
            var parameter = CreateOutputParameter(command, name);
            command.Parameters.Add(parameter);
            return parameter;
        }

        /// <summary>
        ///     Add a parameter for the return value with the given name and type
        /// </summary>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IDbDataParameter AddReturnValueParameter([NotNull] this IDbCommand command,
            [NotNull] string name, DbType type = DbType.AnsiString)
        {
            var parameter = CreateReturnValueParameter(command, name);
            command.Parameters.Add(parameter);
            return parameter;
        }

        public static IDbDataParameter AddBiDirectionalParameter([NotNull] this IDbCommand command,
            [NotNull] string name, object value, DbType type = DbType.AnsiString)
        {
            var parameter = CreateBiDirectionalParameter(command, name, value);
            command.Parameters.Add(parameter);
            return parameter;
        }

        // Creating parameters in bulk.

        /// <summary>
        ///     Add a named argument for every key/value pair in the arguments dictionary
        /// </summary>
        /// <param name="command"></param>
        /// <param name="arguments"></param>
        public static void AddParameters([NotNull] this IDbCommand command,
            [NotNull] Dictionary<string, object> arguments)
        {
            foreach (var pair in arguments)
                AddParameter(command, pair.Key, pair.Value);
        }

        // Creating commands

        /// <summary>
        ///     Create a command with the given commandText
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static IDbCommand CreateCommand(this IDbConnection connection, string commandText)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            return command;
        }

        /// <summary>
        ///     Create a command with the given commandText and the given input only named arguments
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static IDbCommand CreateCommand(this IDbConnection connection, string commandText,
            Dictionary<string, object> arguments)
        {
            var command = CreateCommand(connection, commandText);
            command.AddParameters(arguments);
            return command;
        }

        // Fetching data

        /// <summary>
        ///     (Lazily) fetch an IEnumerable of T using a transformation function from IDataRecord to T
        /// </summary>
        /// <param name="command"></param>
        /// <param name="forEachRow"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> Fetch<T>(this IDbCommand command, Func<IDataRecord, T> forEachRow)
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read()) yield return forEachRow(reader);
            }
        }

        /// <summary>
        ///     (Lazily) fetch an IEnumerable of IDataRecord
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static IEnumerable<IDataRecord> Fetch(this IDbCommand command)
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read()) yield return reader;
            }
        }

        /// <summary>
        ///     Execute the command and collect the results in a DataTable
        /// </summary>
        /// <param name="command"></param>
        /// <param name="loadOption"></param>
        /// <param name="errorHandler"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(this IDbCommand command, LoadOption loadOption = LoadOption.Upsert,
            FillErrorEventHandler errorHandler = null)
        {
            var table = new DataTable();
            using (var reader = command.ExecuteReader())
            {
                table.Load(reader, loadOption, errorHandler);
            }

            return table;
        }

        /// <summary>
        ///     Execute the command and collect the results in a DataSet
        /// </summary>
        /// <param name="command"></param>
        /// <param name="loadOption"></param>
        /// <param name="errorHandler"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(this IDbCommand command, LoadOption loadOption = LoadOption.Upsert,
            FillErrorEventHandler errorHandler = null)
        {
            var set = new DataSet();
            using (var reader = command.ExecuteReader())
            {
                set.Load(reader, loadOption, errorHandler);
            }

            return set;
        }

        /// <summary>
        ///     Execute the command and return the first value of the first row returned as an instance of T
        /// </summary>
        /// <param name="command"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ExecuteScalar<T>(this IDbCommand command)
        {
            var o = command.ExecuteScalar();
            return (T) Convert.ChangeType(o, typeof(T));
        }
    }
}