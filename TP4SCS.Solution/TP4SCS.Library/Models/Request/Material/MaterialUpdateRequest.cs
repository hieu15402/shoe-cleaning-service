using TP4SCS.Library.Models.Request.AssetUrl;

namespace TP4SCS.Library.Models.Request.Material
{
    public class MaterialUpdateRequest
    {
        public int[] BranchId { get; set; } = Array.Empty<int>();
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string? Status { get; set; }

        public List<AssetUrlRequest> AssetUrls { get; set; } = new List<AssetUrlRequest>();
    }
}
