using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class BinaryFileReader 
    {
        internal OperationStatus Execute(string directory)
        {
            return Store.GetInstance().BinaryFileReader(directory);
        }
    }
}