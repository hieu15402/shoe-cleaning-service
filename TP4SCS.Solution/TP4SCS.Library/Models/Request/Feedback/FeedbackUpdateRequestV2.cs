using TP4SCS.Library.Models.Request.AssetUrl;

namespace TP4SCS.Library.Models.Request.Feedback
{
    public class FeedbackUpdateRequestV2
    {
        public string? Content { get; set; }
        public List<AssetUrlRequest> AssetUrls { get; set; } = new List<AssetUrlRequest>();
    }
}
