using ApiInterface.Exceptions;
using ApiInterface.InternalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiInterface.Processors
{
    internal class ProcessorFactory
    {
        internal static IProcessor Create(Request request)
        {
            // Validates the type of request for SQL-Sentences
            if (request.RequestType is RequestType.SQLSentence)
            {
                return new SQLSentenceProcessor(request);
            }

            // Server verification 
            Console.WriteLine($"\n:( \n!Error : Something went wrong...");

            // Throws an exception if the request type is unknown
            throw new UnknowRequestTypeException();
        }
    }
}
