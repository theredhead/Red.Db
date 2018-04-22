using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using General;

namespace DataAccess
{
    /// <summary>
    /// Record is a low level-ish type that can be used to represent a record in any table
    /// and can be transparently loaded and saved using IDatabase
    /// </summary>
    public class Record : IDataRecordLoadable
    {
        private string[] _fieldNames;
        private readonly Dictionary<int,object> _original = new Dictionary<int, object>();
        private readonly Dictionary<int,object> _modified = new Dictionary<int, object>();

        public bool IsNew => _modified.Keys.Count == 0;
        public bool IsModified => _modified.Keys.Count > 0;
        
        public object this[string columnName]
        {
            get => this[GetOrdinal(columnName)];
            set => _modified[GetOrdinal(columnName)] = value;
        }

        protected T TryGet<T>(string columnName, T ifNotSet =  default(T))
        {
            try
            {
                return (T) Convert.ChangeType(this[columnName], typeof(T));
            }
            catch
            {
                return ifNotSet;
            }
        }
        
        private int GetOrdinal(string columnName)
        {
            return Array.IndexOf(_fieldNames, columnName);
        }
        
        protected virtual object this[int columnIndex]
        {
            get
            {
                if (columnIndex > _fieldNames.Length)
                    throw new ArgumentOutOfRangeException();

                return _modified.ContainsKey(columnIndex)
                    ? _modified[columnIndex]
                    : _original[columnIndex];
            }
            set => _modified[columnIndex] = value;
        }

        public virtual void Load(IDataRecord record)
        {
            _original.Clear();
            _modified.Clear();

            var fieldNames = new List<string>();
            for (var index = 0; index < record.FieldCount; index++)
            {
                _original[index] = record[index];
                fieldNames.Add(record.GetName(index));
            }

            _fieldNames = fieldNames.ToArray();
        }
    }
}