namespace ApiInterface.InternalModels
{
    internal enum RequestType // Supported request types 
    { 
        SQLSentence = 0  
    }

    internal class Request // Request object
    {
        public required RequestType RequestType { get; set; } // Defines the type of request

        public required string RequestBody { get; set; } // Contains request body and data
    }
}
