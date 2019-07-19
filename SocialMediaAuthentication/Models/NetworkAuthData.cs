using Newtonsoft.Json;

namespace SocialMediaAuthentication.Models
{
    public class NetworkAuthData
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("pin")]
        public string Pin { get; set; } = string.Empty;

        [JsonProperty("Id")]
        public string Id { get; set; }
    }
}
