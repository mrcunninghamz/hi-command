using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Lambda;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Model;
using Amazon.Runtime;
using Amazon.Util;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Core.SlackApi;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HiCommand.Master
{
    public class Function
    {
        private ILambdaContext _context;
        /// <summary>
        /// The intro method that deligates tasks to other lambdas
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            _context = context;
            var order = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(input.Body);
            
            var response = await RenderOrder(order);
            return response;
        }

        private async Task<APIGatewayProxyResponse> RenderOrder(IReadOnlyDictionary<string, StringValues> order)
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
                    response.Body = await CreateDebugBody(order);
                    break;

                case "quote":
                    await GetQuote(order).ConfigureAwait(false);
                    response.Body = "One Moment...";
                    break;

            }

            return response;
        }

        private async Task<string> CreateDebugBody(IReadOnlyDictionary<string, StringValues> order)
        {
            var payload = new Payload
            {
                Channel = order["channel_id"],
                Username = "Hi-Command",
                Text = JsonConvert.SerializeObject(order)
            };

            return JsonConvert.SerializeObject(payload);
        }

        private async Task GetQuote(IReadOnlyDictionary<string, StringValues> order)
        {
            var slackMessage = new SlackMessage
            {
                ChannelId = order["channel_id"],
                Text = order["text"],
                ResponseUrl = order["response_url"]
            };

            //var client = new AmazonLambdaClient();


            //var request = new InvokeRequest()
            //{
            //    FunctionName = "HiCommand_Quote",
            //    Payload = JsonConvert.SerializeObject(slackMessage),
            //    InvocationType = InvocationType.RequestResponse
            //};

            //await client.InvokeAsync(request);

            var payload = new Payload
            {
                Channel = slackMessage.ChannelId,
                Username = "Hi-Command",
                Text = JsonConvert.SerializeObject(slackMessage)
            };

            using (var client = new HttpClient())
            {
                await client.PostAsync(slackMessage.ResponseUrl, new StringContent(JsonConvert.SerializeObject(payload)));
            }

        }
    }
}
