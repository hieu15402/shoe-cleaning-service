using TP4SCS.Library.Models.Request.AssetUrl;

namespace TP4SCS.Library.Models.Request.Feedback
{
    public class FeedbackRequest
    {
        public int OrderItemId { get; set; }
        public decimal Rating { get; set; }

        public string? Content { get; set; } = null;
        public List<AssetUrlRequest> AssetUrls { get; set; } = new List<AssetUrlRequest>();
    }
}
