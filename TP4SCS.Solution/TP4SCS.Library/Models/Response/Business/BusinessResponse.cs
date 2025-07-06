using System.Text.Json.Serialization;
using TP4SCS.Library.Models.Response.PackSubscription;

namespace TP4SCS.Library.Models.Response.BusinessProfile
{
    public class BusinessResponse
    {
        public int Id { get; set; }

        public int OwnerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public decimal Rating { get; set; }

        public int TotalOrder { get; set; }

        public int PendingAmount { get; set; }

        public int ProcessingAmount { get; set; }

        public int FinishedAmount { get; set; }

        public int CanceledAmount { get; set; }

        public int ToTalServiceNum { get; set; }

        public DateOnly CreatedDate { get; set; }

        public DateTime RegisteredTime { get; set; }

        public DateTime ExpiredTime { get; set; }

        public bool IsIndividual { get; set; }

        public bool IsMaterialSupported { get; set; }

        public bool IsLimitServiceNum { get; set; }

        public string Status { get; set; } = string.Empty;

        [JsonIgnore]
        public List<PackSubscriptionResponse>? PackSubscriptions { get; set; } = null;

        [JsonPropertyName("packSubscriptions")]
        public List<PackSubscriptionResponse>? PackSubscriptionsSerialized
        {
            get => PackSubscriptions?.Any() == true ? PackSubscriptions : null;
            set => PackSubscriptions = value;
        }

    }
}
