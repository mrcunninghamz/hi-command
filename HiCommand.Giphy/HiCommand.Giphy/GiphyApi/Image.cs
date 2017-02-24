using Newtonsoft.Json;

namespace HiCommand.Giphy.GiphyApi
{
    public class Image
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}