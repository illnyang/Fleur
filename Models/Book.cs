using Newtonsoft.Json;

namespace Fleur.Models
{
    public class Book
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("isbn")]
        public string Isbn { get; set; }

        [JsonProperty("popularity_count")]
        public long PopularityCount { get; set; }

        [JsonProperty("edition")]
        public string Edition { get; set; }

        [JsonProperty("authors")]
        public string Authors { get; set; }

        [JsonProperty("hidden")]
        public bool Hidden { get; set; }

        [JsonProperty("priority")]
        public long Priority { get; set; }

        [JsonProperty("announcement")]
        public string Announcement { get; set; }

        [JsonProperty("premium_popularity_count")]
        public long PremiumPopularityCount { get; set; }

        [JsonProperty("publisher")]
        public string Publisher { get; set; }

        [JsonProperty("released")]
        public long Released { get; set; }

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("pages")]
        public long[] Pages { get; set; }

        [JsonProperty("font_color")]
        public string FontColor { get; set; }

        [JsonProperty("part_hidden")]
        public bool PartHidden { get; set; }

        [JsonProperty("last_page")]
        public long? LastPage { get; set; }

        [JsonProperty("tags")]
        public object Tags { get; set; }

        [JsonProperty("wiki")]
        public object[] Wiki { get; set; }

        [JsonProperty("grades")]
        public string[] Grades { get; set; }

        [JsonProperty("grades_number")]
        public string GradesNumber { get; set; }

        [JsonProperty("series")]
        public Series Series { get; set; }
    }

    public class Series
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("priority")]
        public long Priority { get; set; }
    }
}
