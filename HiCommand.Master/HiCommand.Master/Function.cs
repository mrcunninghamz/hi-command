using System;
using System.Collections.Generic;
using System.Linq;
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

        private string _body;
        /// <summary>
        /// The intro method that deligates tasks to other lambdas
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            _context = context;
            _body = input.Body;

            var order = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(_body);
            
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

            var slackMessage = CreateSlackMessage(order);

            switch (command)
            {
                case "":
                    response.Body = $"hello there, {slackMessage.Username}!";
                    break;

                case "debug":
                    response.Body = await CreateDebugBody(slackMessage);
                    break;

                case "quote":
                    await CallLambda(slackMessage, "HiCommand_Quote");
                    response.Body = await createLetMeThink(slackMessage);
                    break;

                case "animate":
                    await CallLambda(slackMessage, "HiCommand_Giphy");
                    response.Body = await createLetMeThink(slackMessage);
                    break;

            }

            return response;
        }

        private async Task<string> CreateDebugBody(SlackMessage slackMessage)
        {
            var payload = new Payload
            {
                Channel = slackMessage.ChannelId,
                Username = "Hi-Command",
                Text = JsonConvert.SerializeObject(_body)
            };

            return JsonConvert.SerializeObject(payload);
        }

        private async Task<string> createLetMeThink(SlackMessage slackMessage)
        {
            var payload = new Payload
            {
                ResponseType = "in_channel",
                Channel = slackMessage.ChannelId,
                Username = "Hi-Command",
                Text = $"Thanks {slackMessage.Username}, let me think about that."
            };

            return JsonConvert.SerializeObject(payload);
        }

        private SlackMessage CreateSlackMessage(IReadOnlyDictionary<string, StringValues> order)
        {
            var slackMessage = new SlackMessage
            {
                Token = order["token"],
                TeamId = order["team_id"],
                TeamDomain = order["team_domain"],
                ChannelId = order["channel_id"],
                ChannelName = order["channel_name"],
                UserId = order["user_id"],
                Username = order["user_name"],
                Text = order["text"]
            };

            return slackMessage;
        }

        private async Task CallLambda(SlackMessage slackMessage, string functionName)
        {
            using (var awsClient = new AmazonLambdaClient())
            {
                var request = new InvokeRequest()
                {
                    FunctionName = functionName,
                    Payload = JsonConvert.SerializeObject(slackMessage),
                    InvocationType = InvocationType.Event
                };

                await awsClient.InvokeAsync(request);
            }
        }
    }
}
