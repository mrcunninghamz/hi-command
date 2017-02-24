using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Core.SlackApi
{
    public class Payload
    {

        [JsonProperty("response_type")]
        public string ResponseType { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("attachments")]
        public IEnumerable<Attachment> Attachments { get; set; }
    }

    public class Attachment
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }
    }
}
