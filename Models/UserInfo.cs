using System;
using Newtonsoft.Json;

namespace Fleur.Models
{
    public class UserInfo
    {
        [JsonProperty("had_dcb")]
        public bool HadDcb { get; set; }

        [JsonProperty("subscriptions_counter")]
        public long SubscriptionsCounter { get; set; }

        [JsonProperty("exercises_counter")]
        public long ExercisesCounter { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("subscription_expiration_grade_name")]
        public string SubscriptionExpirationGradeName { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("avatar_large")]
        public Uri AvatarLarge { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("provider")]
        public object Provider { get; set; }

        [JsonProperty("auth_token")]
        public string AuthToken { get; set; }

        [JsonProperty("referral_code")]
        public string ReferralCode { get; set; }

        [JsonProperty("roles_list")]
        public string RolesList { get; set; }

        [JsonProperty("is_premium")]
        public bool IsPremium { get; set; }

        [JsonProperty("has_avatar")]
        public bool HasAvatar { get; set; }

        [JsonProperty("has_phone")]
        public bool HasPhone { get; set; }

        [JsonProperty("subscription_expiration_time")]
        public long SubscriptionExpirationTime { get; set; }

        [JsonProperty("should_receive_referral_prize")]
        public bool ShouldReceiveReferralPrize { get; set; }

        [JsonProperty("can_commenting")]
        public bool CanCommenting { get; set; }

        [JsonProperty("active_subscriptions")]
        public ActiveSubscription[] ActiveSubscriptions { get; set; }

        [JsonProperty("pending_subscriptions")]
        public object[] PendingSubscriptions { get; set; }

        [JsonProperty("user_series")]
        public UserSeries UserSeries { get; set; }

        [JsonProperty("rodo_approve")]
        public RodoApprove RodoApprove { get; set; }

        [JsonProperty("popups")]
        public object[] Popups { get; set; }
    }

    public class ActiveSubscription
    {
        [JsonProperty("expire_in")]
        public long ExpireIn { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("grade")]
        public string Grade { get; set; }
    }

    public class RodoApprove
    {
        [JsonProperty("general")]
        public bool General { get; set; }

        [JsonProperty("device")]
        public bool Device { get; set; }

        [JsonProperty("comment")]
        public bool Comment { get; set; }
    }

    public class UserSeries
    {
        [JsonProperty("data")]
        public string Data { get; set; }
    }
}
