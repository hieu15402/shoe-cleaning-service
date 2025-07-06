using TP4SCS.Library.Models.Request.AssetUrl;

namespace TP4SCS.Library.Models.Response.Service
{
    public class ServiceCreateResponse
    {
        public required int[] BranchId { get; set; }
        public int CategoryId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }
        public decimal? NewPrice { get; set; }
        public string Status { get; set; } = null!;
        public List<AssetUrlRequest> AssetUrls { get; set; } = new List<AssetUrlRequest>();
    }
}
