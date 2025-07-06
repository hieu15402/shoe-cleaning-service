using TP4SCS.Library.Models.Response.AssetUrl;
using TP4SCS.Library.Models.Response.BranchMaterial;

namespace TP4SCS.Library.Models.Response.Material
{
    public class MaterialResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }
        public string Status { get; set; } = null!;
        public List<AssetUrlResponse>? AssetUrls { get; set; }
        public List<BranchMaterialResponse> BranchMaterials { get; set; } = new List<BranchMaterialResponse>();
    }
}
