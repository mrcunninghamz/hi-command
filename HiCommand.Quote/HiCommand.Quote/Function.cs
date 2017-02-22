using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization;
using HiCommand.Quote.GeniusApi.Models;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Core.SlackApi;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializerAttribute(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HiCommand.Quote
{
    public class Function
    {
        /// <summary>
        /// searches Genius for a match against the key words and returns a quote
        /// </summary>
        /// <param name="order"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(SlackMessage order, ILambdaContext context)
        {
            await GetQuote(order);
        }

        public async Task GetQuote(SlackMessage order)
        {
            var commands = order.Text.Split(' ');
            var artist = string.Join(" ", commands.Skip(1));
            var channelId = order.ChannelId;
            var responseUrl = order.ResponseUrl;

            string songTitle;
            string songUrl;
            string artistFullName;
            var random = new Random();
            using (var client = new HttpClient())
            {
                var clientToken = Environment.GetEnvironmentVariable("Genius_Token");
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
                ResponseType = "in_channel",
                Channel = channelId,
                Username = "Hi-Command",
                Text = $"\"{lyrics}\" \n - {artistFullName}, <{songUrl}|{songTitle}>"
            };

            using (var client = new HttpClient())
            {
                await client.PostAsync(responseUrl, new StringContent(JsonConvert.SerializeObject(payload)));
            }
        }
    }
}
