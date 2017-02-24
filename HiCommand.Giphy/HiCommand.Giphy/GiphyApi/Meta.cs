using Newtonsoft.Json;

namespace HiCommand.Giphy.GiphyApi
{
    public class Meta
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("msg")]
        public string Message { get; set; }
    }
}