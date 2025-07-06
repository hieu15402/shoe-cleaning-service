using TP4SCS.Library.Models.Response.Branch;
using TP4SCS.Library.Models.Response.Material;
using TP4SCS.Library.Models.Response.Order;
using TP4SCS.Library.Models.Response.Service;

namespace TP4SCS.Library.Models.Response.OrderDetail
{
    public class OrderDetailResponse
    {
        public int Id { get; set; }

        public OrderResponse Order { get; set; } = null!;

        public BranchResponse Branch { get; set; } = null!;

        public ServiceResponse? Service { get; set; }

        public MaterialResponse? Material { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string Status { get; set; } = null!;
    }
}
