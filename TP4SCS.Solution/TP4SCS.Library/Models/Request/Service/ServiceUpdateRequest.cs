using TP4SCS.Library.Models.Request.AssetUrl;
using TP4SCS.Library.Models.Request.Process;

namespace TP4SCS.Library.Models.Request.Service
{
    public class ServiceUpdateRequest
    {
        public int[] BranchId { get; set; } = Array.Empty<int>();
        public int CategoryId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? Status { get; set; }

        public decimal? NewPrice { get; set; }
        public List<AssetUrlRequest> AssetUrls { get; set; } = new List<AssetUrlRequest>();
        public List<ProcessUpdateRequest> ServiceProcesses { get; set; } = new List<ProcessUpdateRequest>();
    }
}
