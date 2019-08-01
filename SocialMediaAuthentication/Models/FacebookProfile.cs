using Newtonsoft.Json;

namespace SocialMediaAuthentication.Models
{
    public class Data
    {
        [JsonProperty("is_silhouette")]
        public bool IsSilhouette { get; set; }
        public int Height { get; set; }
        public string Url { get; set; }
        public int Width { get; set; }
    }

    public class Picture
    {
        public Data Data { get; set; }
    }

    public class FacebookProfile
    {
        public string Email { get; set; }
        public string Id { get; set; }
        public Picture Picture { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
    }

}