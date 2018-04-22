using System.Data;

namespace DataAccess
{
    public interface IDataRecordLoadable
    {
        void Load(IDataRecord record);
    }
}