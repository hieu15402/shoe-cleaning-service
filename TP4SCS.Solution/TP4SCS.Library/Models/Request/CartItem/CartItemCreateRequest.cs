namespace TP4SCS.Library.Models.Request.CartItem
{
    public class CartItemCreateRequest
    {
        public int AccountId { get; set; }
        public int ServiceId { get; set; }
        public List<int> MaterialIds { get; set; } = new List<int>();
        public int BranchId { get; set; }
    }
}
