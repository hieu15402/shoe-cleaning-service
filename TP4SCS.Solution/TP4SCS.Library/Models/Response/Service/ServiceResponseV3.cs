using TP4SCS.Library.Models.Response.AssetUrl;
using TP4SCS.Library.Models.Response.BranchService;
using TP4SCS.Library.Models.Response.Category;
using TP4SCS.Library.Models.Response.Material;
using TP4SCS.Library.Models.Response.Process;
using TP4SCS.Library.Models.Response.Promotion;

namespace TP4SCS.Library.Models.Response.Service
{
    public class ServiceResponseV3
    {
        public int Id { get; set; }

        public ServiceCategoryResponse Category { get; set; } = null!;

        public string Name { get; set; } = null!;
        public int BusinessId { get; set; }
        public string BusinessName { get; set; } = null!;
        public int? BusinessRank { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public decimal Rating { get; set; }

        public DateTime CreateTime { get; set; }

        public int OrderedNum { get; set; }

        public int FeedbackedNum { get; set; }

        public string Status { get; set; } = null!;

        public PromotionResponse? Promotion { get; set; }

        public List<AssetUrlResponse>? AssetUrls { get; set; }
        public List<ProcessResponse>? ServiceProcesses { get; set; }
        public List<BranchServiceResponse> BranchServices { get; set; } = new List<BranchServiceResponse>();
        public List<MaterialResponse> Materials { get; set; } = new List<MaterialResponse>();
    }
}
