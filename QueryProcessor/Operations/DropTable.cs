using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class DropTable
    {
        internal OperationStatus Execute(string TableName)
        {
            return Store.GetInstance().DropTable(TableName);
        }
    }
}
