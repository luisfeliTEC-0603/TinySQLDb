using ApiInterface.InternalModels;
using Entities;

namespace ApiInterface.Models 
{
    internal class Response // Response object
    {
        public required Request Request { get; set; } // Original request that request the response

        public required OperationStatus Status { get; set; } // Status of the operation

        public required string ResponseBody { get; set; } // Content of the response 
    }
}
