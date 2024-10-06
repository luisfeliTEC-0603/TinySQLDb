using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class Update
    {
        internal OperationStatus Execute(string tableName, string[] selectedColumns,  string whereClause)
        {

            return Store.GetInstance().Update(tableName, selectedColumns, whereClause);
        }
    }
}
