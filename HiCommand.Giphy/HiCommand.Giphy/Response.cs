using Newtonsoft.Json;

namespace HiCommand.Giphy.GiphyApi
{
    public class Response
    {
        [JsonProperty("data")]
        public Data Data { get; set; }
    }
}