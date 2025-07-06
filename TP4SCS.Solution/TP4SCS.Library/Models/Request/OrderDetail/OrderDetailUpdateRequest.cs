using TP4SCS.Library.Models.Request.AssetUrl;

namespace TP4SCS.Library.Models.Request.OrderDetail
{
    public class OrderDetailUpdateRequest
    {
        public string? ProcessState { get; set; }
        public virtual ICollection<AssetUrlRequest> AssetUrls { get; set; } = new List<AssetUrlRequest>();
    }
}
