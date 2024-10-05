using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class CreateDataBase
    {
        internal OperationStatus Execute(string dbName)
        {
            return Store.GetInstance().CreateDataBase(dbName);
        }
    }
}
