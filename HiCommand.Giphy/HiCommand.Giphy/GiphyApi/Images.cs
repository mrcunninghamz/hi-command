using Newtonsoft.Json;

namespace HiCommand.Giphy.GiphyApi
{
    public class Images
    {

        [JsonProperty("original")]
        public Image Original { get; set; }
    }
}