﻿using System.Net.Sockets;
using System.Net;
using System.Text;
using ApiInterface.InternalModels;
using System.Text.Json;
using ApiInterface.Exceptions;
using ApiInterface.Processors;
using ApiInterface.Models;

namespace ApiInterface
{
    public class Server
    {
        // Server IP endPoint & port specification
        private static IPEndPoint serverEndPoint = new(IPAddress.Loopback, 40404);
        private static int supportedParallelConnections = 1; // Set limits of the parallel connections

        public static async Task Start() // Asynchronously starts the Server
        {
            // Initializes a TCP listener socket bound to the specified endpoint
            using Socket listener = new(serverEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(serverEndPoint);
            listener.Listen(supportedParallelConnections);

            // Server verification
            Console.WriteLine($"\n--- SERVER INITIALIZED AT ENDPOINT : [{serverEndPoint.ToString()}] ---\n");

            while (true) // Continuously listens for incoming client connections
            {
                // Accepts client connection and returns a socket handler 
                var handler = await listener.AcceptAsync();
                try
                {
                    // Retrieves the raw message sent by the client through the socket
                    var rawMessage = GetMessage(handler);

                    // Converts the raw message into a request object
                    var requestObject = ConvertToRequestObject(rawMessage);

                    // Processes the request object and generates a corresponding response
                    var response = ProcessRequest(requestObject);

                    // Sends the generated response back to the client
                    SendResponse(response, handler);
                }
                catch (Exception ex)
                {
                    // Server verification 
                    Console.WriteLine($"\n:( \n!Error : {ex}...");

                    // Exception is raised
                    await SendErrorResponse("Unknown exception", handler);
                }
                finally
                {
                    // Socket handler is closed 
                    handler.Close();
                }
            }
        }

        private static string GetMessage(Socket handler)
        {
            // Creates a NetworkStream for reading data from the socket
            using (NetworkStream stream = new NetworkStream(handler))
            using (StreamReader reader = new StreamReader(stream))
            {
                // Reads a line from the stream and returns it, or returns an empty string if null
                return reader.ReadLine() ?? String.Empty;
            }
        }

        private static Request ConvertToRequestObject(string rawMessage) 
        {
            // Deserializes a raw message into a Request object
            return JsonSerializer.Deserialize<Request>(rawMessage) ?? throw new InvalidRequestException();
        }

        private static Response ProcessRequest(Request requestObject)
        {
            // Request is processes 
            var processor = ProcessorFactory.Create(requestObject);
            return processor.Process();
        }

        private static void SendResponse(Response response, Socket handler)
        {
            // Creates a NetworkStream for sending data through the socket 
            using (NetworkStream stream = new NetworkStream(handler))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                // Serializes the response to JSON and sends it to the client
                writer.WriteLine(JsonSerializer.Serialize(response));
            }
        }

        private static Task SendErrorResponse(string reason, Socket handler)
        {
            throw new NotImplementedException(); // Throws an exception as the functionality is not implemented
        }        
    }
}