using Entities;
using StoreDataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryProcessor.Operations
{
    internal class Select
    {
        public OperationStatus Execute(string directory, string[] columns, string whereClause, string OrderClause)
        {
            return Store.GetInstance().Select(directory, columns, whereClause, OrderClause);
        }
    }
}
