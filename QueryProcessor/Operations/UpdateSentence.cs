using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class UpdateSentence
    {
        // Sobrecarga para cuando existe una cláusula WHERE (3 parámetros)
        internal OperationStatus Execute(string tableName, string[] selectedColumns,  string whereClause)
        {

            return Store.GetInstance().UpdateSentence(tableName, selectedColumns, whereClause);
        }
    }
}
