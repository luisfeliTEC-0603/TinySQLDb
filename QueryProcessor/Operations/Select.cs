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
        public OperationStatus Execute(string column, string whereClause, string OrderClause)
        {
            // This is only doing the query but not returning results.
            return Store.GetInstance().Select();
        }
    }
}
