using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class Delete
    {
        internal OperationStatus Execute(string directory, string whereClause)
        {
            return Store.GetInstance().Delete(directory, whereClause);
        }
    }
}
