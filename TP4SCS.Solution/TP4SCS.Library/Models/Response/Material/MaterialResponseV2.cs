using TP4SCS.Library.Models.Response.AssetUrl;

namespace TP4SCS.Library.Models.Response.Material
{
    public class MaterialResponseV2
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Status { get; set; } = null!;
        public decimal Price { get; set; }
        public virtual ICollection<AssetUrlResponse> AssetUrls { get; set; } = new List<AssetUrlResponse>();
    }
}
