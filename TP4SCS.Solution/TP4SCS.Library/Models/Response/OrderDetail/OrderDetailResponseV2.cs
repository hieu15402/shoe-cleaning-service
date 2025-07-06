using TP4SCS.Library.Models.Response.AssetUrl;
using TP4SCS.Library.Models.Response.Branch;
using TP4SCS.Library.Models.Response.Feedback;
using TP4SCS.Library.Models.Response.Material;
using TP4SCS.Library.Models.Response.Service;

namespace TP4SCS.Library.Models.Response.OrderDetail
{
    public class OrderDetailResponseV2
    {
        public int Id { get; set; }
        public BranchResponse Branch { get; set; } = null!;

        public ServiceResponseV2 Service { get; set; } = null!;

        public List<MaterialResponseV2> Materials { get; set; } = new List<MaterialResponseV2>();

        public virtual FeedbackResponse? Feedback { get; set; }
        public virtual ICollection<AssetUrlResponse> AssetUrls { get; set; } = new List<AssetUrlResponse>();
        public string? ProcessState { get; set; }
        public string? Note { get; set; }

        public decimal Price { get; set; }
    }
}
