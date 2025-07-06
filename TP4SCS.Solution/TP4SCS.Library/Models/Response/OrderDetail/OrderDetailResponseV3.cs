using TP4SCS.Library.Models.Response.Branch;
using TP4SCS.Library.Models.Response.Service;

namespace TP4SCS.Library.Models.Response.OrderDetail
{
    public class OrderDetailResponseV3
    {
        public int Id { get; set; }

        public BranchResponse Branch { get; set; } = null!;

        public ServiceResponseV2 Service { get; set; } = null!;
    }
}
