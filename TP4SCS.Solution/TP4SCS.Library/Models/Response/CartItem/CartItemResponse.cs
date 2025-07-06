using TP4SCS.Library.Models.Response.Material;

namespace TP4SCS.Library.Models.Response.CartItem
{
    public class CartItemResponse
    {
        public int Id { get; set; }

        public int CartId { get; set; }
        public int BranchId { get; set; }
        public int ServiceId { get; set; }
        public List<MaterialResponseV3> Materials { get; set; } = new List<MaterialResponseV3>();
        public string ServiceName { get; set; } = null!;
        public string ServiceStatus { get; set; } = null!;
        public decimal Price { get; set; }
    }
}
