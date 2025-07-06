using TP4SCS.Library.Models.Response.AssetUrl;
using TP4SCS.Library.Models.Response.Order;
using TP4SCS.Library.Models.Response.OrderDetail;

namespace TP4SCS.Library.Models.Response.Feedback
{
    public class FeedbackResponse
    {
        public int Id { get; set; }

        public OrderDetailResponseV3 OrderItem { get; set; } = null!;
        public OrderFeedbackResponse Order { get; set; } = null!;

        public decimal Rating { get; set; }

        public string? Content { get; set; }
        public string? Reply { get; set; }
        public DateTime CreatedTime { get; set; }

        public bool IsValidContent { get; set; }
        public bool IsAllowedUpdate { get; set; }

        public bool IsValidAsset { get; set; }

        public string Status { get; set; } = null!;

        public virtual ICollection<AssetUrlResponse> AssetUrls { get; set; } = new List<AssetUrlResponse>();
    }
}
