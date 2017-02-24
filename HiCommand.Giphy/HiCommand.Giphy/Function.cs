using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using HiCommand.Giphy.GiphyApi;
using Newtonsoft.Json;
using Core.SlackApi;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HiCommand.Giphy
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes the command "annimate"
        /// </summary>
        /// <param name="order">The slack message</param>
        /// <param name="context">The labmda context</param>
        /// <returns></returns>
        public async Task FunctionHandler(SlackMessage order, ILambdaContext context)
        {
            var commands = order.Text.Split(' ');
            var phrase = string.Join(" ", commands.Skip(1));

            var channelId = order.ChannelId;
            var responseUrl = order.ResponseUrl;

            var gifUrl = string.Empty;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://api.giphy.com");
                var response = client.GetAsync($"v1/gifs/translate?s={phrase}&api_key=dc6zaTOxFJmzC");
                var giphyResponse = JsonConvert.DeserializeObject<Response>(response.Result.Content.ReadAsStringAsync().Result);
                gifUrl = giphyResponse.Data.Images.Original.Url;
            }

            var payload = new Payload
            {
                ResponseType = "in_channel",
                Channel = channelId,
                Username = "Hi-Command",
                Attachments = new List<Attachment> {
                    new Attachment
                    {
                        ImageUrl = gifUrl
                    }
                }
            };

            using (var client = new HttpClient())
            {
                await client.PostAsync(responseUrl, new StringContent(JsonConvert.SerializeObject(payload)));
            }
        }
    }
}
