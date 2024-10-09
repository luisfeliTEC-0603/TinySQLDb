using ApiInterface.InternalModels;
using ApiInterface.Models;
using Entities;
using QueryProcessor;

namespace ApiInterface.Processors
{
    internal class SQLSentenceProcessor(Request request) : IProcessor 
    {
        // Request associated with this processor
        public Request Request { get; } = request;

        public Response Process()
        {
            // Extracts the SQL sentence from the request body
            var sentence = this.Request.RequestBody;

            // Executes the SQL sentence using the SQLQueryProcessor and retrieves the result
            var result = SQLQueryProcessor.Execute(sentence);

            // Converts the result of the SQL execution into a Response object
            var response = this.ConvertToResponse(result);

            // Returns the constructed response
            return response;
        }

        private Response ConvertToResponse(OperationStatus result)
        {
            return new Response // Response Object is created as a response to the operation
            {
                Status = result,
                Request = this.Request,
                ResponseBody = result == OperationStatus.Success ? "The operation has been a succes :D" : "Something wrong happend :("
            };
        }
    }
}
