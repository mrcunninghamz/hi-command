using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HiCommand
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            var order = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(input.Body);

            var response = RenderOrder(order);
            return response;
        }

        private APIGatewayProxyResponse RenderOrder(IReadOnlyDictionary<string, StringValues> order)
        {

            var response = new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = "An error occured and I don't know what to do with myself"
            };
            
            var commands = order["text"].ToString().Split(' ');
            var command = commands[0] ?? string.Empty;

            switch (command)
            {
                case "":
                    response.Body = $"hello there, {order["user_name"]}!";
                    break;
                case "debug":
                    response.Body = CreateDebugBody(order);
                    break;

            }

            return response;
        }

        private string CreateDebugBody(IReadOnlyDictionary<string, StringValues> order)
        {
            var payload = new Payload
            {
                Channel = order["channel_id"],
                Username = "Hi-Command",
                Text = JsonConvert.SerializeObject(order)
            };

            return JsonConvert.SerializeObject(payload);
        }

        //This class serializes into the Json payload required by Slack Incoming WebHooks
        public class Payload
        {
            [JsonProperty("channel")]
            public string Channel { get; set; }

            [JsonProperty("username")]
            public string Username { get; set; }

            [JsonProperty("text")]
            public string Text { get; set; }
        }
    }
}
