using Newtonsoft.Json;

namespace Fleur.Models
{
    public class Exercise
    {
        [JsonProperty("free_premium_part")]
        public string FreePremiumPart { get; set; }

        [JsonProperty("individual")]
        public bool Individual { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("page")]
        public long Page { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("solution")]
        public string Solution { get; set; }

        [JsonProperty("answer")]
        public string Answer { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("book_id")]
        public long BookId { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }

        [JsonProperty("accepted")]
        public bool Accepted { get; set; }

        [JsonProperty("likes")]
        public long Likes { get; set; }

        [JsonProperty("dislikes")]
        public long Dislikes { get; set; }

        [JsonProperty("hidden")]
        public bool Hidden { get; set; }

        [JsonProperty("series")]
        public long Series { get; set; }

        [JsonProperty("natural_sort_order")]
        public long NaturalSortOrder { get; set; }

        [JsonProperty("premium")]
        public bool Premium { get; set; }

        [JsonProperty("base_vote_average")]
        public string BaseVoteAverage { get; set; }

        [JsonProperty("base_vote_quantity")]
        public long BaseVoteQuantity { get; set; }

        [JsonProperty("block_popular")]
        public bool BlockPopular { get; set; }

        [JsonProperty("popularity_vote")]
        public long PopularityVote { get; set; }

        [JsonProperty("vote_count")]
        public long VoteCount { get; set; }

        [JsonProperty("vote_avg")]
        public string VoteAvg { get; set; }

        [JsonProperty("images_generated")]
        public bool ImagesGenerated { get; set; }

        [JsonProperty("permanently_free")]
        public bool PermanentlyFree { get; set; }

        [JsonProperty("ad")]
        public object Ad { get; set; }

        [JsonProperty("popular")]
        public bool Popular { get; set; }

        [JsonProperty("prev")]
        public Next Prev { get; set; }

        [JsonProperty("next")]
        public Next Next { get; set; }

        [JsonProperty("book")]
        public Book Book { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("attachments")]
        public object[] Attachments { get; set; }

        [JsonProperty("wiki_group")]
        public object WikiGroup { get; set; }

        [JsonProperty("tags")]
        public object Tags { get; set; }

        [JsonProperty("shared_warned")]
        public object SharedWarned { get; set; }

        [JsonProperty("announcement")]
        public string Announcement { get; set; }

        [JsonProperty("rating")]
        public Rating Rating { get; set; }

        [JsonProperty("reload_page")]
        public object ReloadPage { get; set; }
    }

    public class Next
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("page")]
        public long Page { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }
    }

    public class Rating
    {
        [JsonProperty("my")]
        public object My { get; set; }

        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("avg")]
        public string Avg { get; set; }
    }

    public class User
    {
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("author")]
        public Author Author { get; set; }
    }

    public class Author
    {
        [JsonProperty("id")]
        public long Id { get; set; }
    }
}
