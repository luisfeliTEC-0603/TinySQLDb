using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class CreateTable
    {
        internal OperationStatus Execute(string directory, string[] columnEntries)
        {
            return Store.GetInstance().CreateTable(directory, columnEntries);
        }
    }
}
