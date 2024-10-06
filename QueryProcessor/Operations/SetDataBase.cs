using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class SetDataBase
    {
        internal OperationStatus Execute(string newDirectory)
        {
            return Store.GetInstance().SetDataBase(newDirectory);
        }
    }
}
