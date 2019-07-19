using Newtonsoft.Json;

namespace SocialMediaAuthentication.Models
{
    public class FacebookProfile
    {
        [JsonProperty("id")]
        public string UserId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }
    }
}