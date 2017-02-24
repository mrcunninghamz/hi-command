using Newtonsoft.Json;

namespace HiCommand.Giphy.GiphyApi
{
    public class Data
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("images")]
        public Images Images { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }

    }
}