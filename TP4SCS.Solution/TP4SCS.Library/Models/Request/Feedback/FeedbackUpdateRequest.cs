namespace TP4SCS.Library.Models.Request.Feedback
{
    public class FeedbackUpdateRequest
    {
        public bool? IsValidContent { get; set; }
        public bool? IsValidAsset { get; set; }
        public string? Status { get; set; }
    }
}
