using Entities;
using StoreDataManager;

namespace QueryProcessor.Operations
{
    internal class CreateTable
    {
        internal OperationStatus Execute(string Directory, string[] Sentence)
        {
            return Store.GetInstance().CreateTable(Directory, Sentence);
        }
    }
}
