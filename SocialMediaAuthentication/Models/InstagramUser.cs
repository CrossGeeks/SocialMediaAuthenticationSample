using System;
using Newtonsoft.Json;

namespace SocialMediaAuthentication.Models
{
    public class InstagramCounts
    {
        public int Media { get; set; }
        public int Follows { get; set; }
        [JsonProperty("followed_by")]
        public int FollowedBy { get; set; }
    }

    public class InstagramData
    {
        public string Id { get; set; }
        public string Username { get; set; }
        [JsonProperty("profile_picture")]
        public string ProfilePicture { get; set; }
        [JsonProperty("full_name")]
        public string FullName { get; set; }
        public string Bio { get; set; }
        public string Website { get; set; }
        [JsonProperty("is_business")]
        public bool IsBusiness { get; set; }
        public InstagramCounts Counts { get; set; }
    }

    public class InstagramMeta
    {
        public int Code { get; set; }
    }

    public class InstagramUser
    {
        public InstagramData Data { get; set; }
        public InstagramMeta Meta { get; set; }
    }
}
