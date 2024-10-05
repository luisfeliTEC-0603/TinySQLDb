using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class SetDataBase
    {
        internal OperationStatus Execute(string NewDirectory)
        {
            return Store.GetInstance().SetDataBase(NewDirectory);
        }
    }
}
