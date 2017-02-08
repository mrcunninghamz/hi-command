using System.Collections.Generic;
using Newtonsoft.Json;

namespace HiCommand.Quote.GeniusApi.Models
{
    public class ApiSearchResponse
    {
        [JsonProperty("meta")]
        public Meta Meta { get; set; }

        [JsonProperty("response")]
        public HitResponse Response { get; set; }

    }

    public class ApiSongResponse
    {
        [JsonProperty("meta")]
        public Meta Meta { get; set; }

        [JsonProperty("response")]
        public SongResponse Response { get; set; }
    }

    public class HitResponse
    {
        [JsonProperty("hits")]
        public IEnumerable<Hit> Hits { get; set; }
    }

    public class SongResponse
    {
        [JsonProperty("song")]
        public Result Song { get; set; }
    }

    public class Hit
    {
        [JsonProperty("index")]
        public string Index { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }


        [JsonProperty("result")]
        public Result Result { get; set; }
    }

    public class Result : GeniusItem
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        
        [JsonProperty("primary_artist")]
        public Artist Artist { get; set; }

    }

    public class Artist : GeniusItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class GeniusItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("api_path")]
        public string ApiPath { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class Meta
    {
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
