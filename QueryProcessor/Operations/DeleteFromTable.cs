using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class DeleteFromTable
    {
        internal OperationStatus Execute(string Directory, string whereClause)
        {
            return Store.GetInstance().DeleteFromTable(Directory, whereClause);
        }
    }
}
