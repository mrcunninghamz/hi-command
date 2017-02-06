using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using HiCommand.GeniusApi.Models;
using HtmlAgilityPack;
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
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
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
                    var artist = string.Join(" ", commands.Skip(1));
                    await GetQuote(artist, order["channel_id"], order["response_url"]);
                    response.Body = $"Thanks {order["user_name"]}! Enjoy.";
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

        private async Task GetQuote(string artist, string channelId, string repsonseUrl)
        {
            string songTitle;
            string songUrl;
            string artistFullName;
            var random = new Random();
            using (var client = new HttpClient())
            {
                var clientToken = "asi8brVVjT1c7_6KFZACtXNV0l2avuESzdVwWcwTGW-mEVHLTINo9AN-3XfQ6zyK";
                client.BaseAddress = new Uri("https://api.genius.com/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", clientToken);

                var search = client.GetAsync($"search?q={artist}");
                var geniusSearchResponse = JsonConvert.DeserializeObject<ApiSearchResponse>(search.Result.Content.ReadAsStringAsync().Result);

                var song = geniusSearchResponse.Response.Hits.Where(x => x.Type.Equals("song")).OrderBy(x => random.Next()).First();

                var songSearch = client.GetAsync(song.Result.ApiPath);
                var songSearchResponse = JsonConvert.DeserializeObject<ApiSongResponse>(songSearch.Result.Content.ReadAsStringAsync().Result);
                songTitle = songSearchResponse.Response.Song.Title;
                songUrl = songSearchResponse.Response.Song.Url;
                artistFullName = songSearchResponse.Response.Song.Artist.Name;
            }

            string lyrics;
            using (var client = new HttpClient())
            {
                var songPage = client.GetAsync(songUrl);
                var html = songPage.Result.Content.ReadAsStringAsync().Result;
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                
                var lyricsNode = htmlDocument.DocumentNode.SelectNodes("//*[contains(@ng-click, 'open()')]").OrderBy(x => random.Next()).First();
                lyrics = lyricsNode.InnerText;
            }



            var payload = new Payload
            {
                Channel = channelId,
                Username = "Hi-Command",
                Text = $"\"{lyrics}\" \n - {artistFullName}, <{songUrl}|{songTitle}>"
            };

            using (var client = new HttpClient())
            {
                await client.PostAsync(repsonseUrl, new StringContent(JsonConvert.SerializeObject(payload)));
            }
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
