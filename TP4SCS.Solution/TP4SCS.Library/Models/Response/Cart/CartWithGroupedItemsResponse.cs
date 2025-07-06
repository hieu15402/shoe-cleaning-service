namespace TP4SCS.Library.Models.Response.Cart
{
    public class CartWithGroupedItemsResponse
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public decimal TotalPrice { get; set; }
        public IEnumerable<dynamic> CartItems { get; set; } = Enumerable.Empty<dynamic>();
    }
}
