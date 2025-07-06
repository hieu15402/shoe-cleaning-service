using TP4SCS.Library.Models.Response.AssetUrl;
using TP4SCS.Library.Models.Response.Promotion;

namespace TP4SCS.Library.Models.Response.Service
{
    public class ServiceResponseV2
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string Status { get; set; } = null!;
        public PromotionResponse? Promotion { get; set; }
        public virtual ICollection<AssetUrlResponse> AssetUrls { get; set; } = new List<AssetUrlResponse>();
    }
}
