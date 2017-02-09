using Newtonsoft.Json;

namespace Core.SlackApi
{
    public class SlackMessage
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("team_id")]
        public string TeamId { get; set; }

        [JsonProperty("team_domain")]
        public string TeamDomain { get; set; }

        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        [JsonProperty("channel_name")]
        public string ChannelName { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("command")]
        public string Command { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("response_url")]
        public string ResponseUrl { get; set; }
    }
}