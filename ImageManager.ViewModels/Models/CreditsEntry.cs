using Newtonsoft.Json;

namespace ImageManager.ViewModels.Models
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
