using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class Insert
    {
        internal OperationStatus Execute(string tableName, string[] columnsData)
        {
            return Store.GetInstance().Insert(tableName, columnsData);
        }
    }
}
