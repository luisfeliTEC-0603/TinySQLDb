using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class Insert
    {
        internal OperationStatus Execute(string TableName, string[] Data)
        {
            return Store.GetInstance().Insert(TableName, Data);
        }
    }
}
