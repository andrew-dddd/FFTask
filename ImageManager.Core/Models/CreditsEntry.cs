using Newtonsoft.Json;

namespace ImageManager.Core.Models
{
    public class CreditsEntry
    {
        [JsonProperty("user_name")]
        public string UserName { get; set; }

        [JsonProperty("user_url")]
        public string UserUrl { get; set; }

        [JsonProperty("photo_url")]
        public string PhotoUrl { get; set; }
    }
}
