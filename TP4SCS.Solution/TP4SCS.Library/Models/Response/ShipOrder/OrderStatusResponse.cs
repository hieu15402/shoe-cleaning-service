namespace TP4SCS.Library.Models.Response.ShipOrder
{
    public class OrderStatusResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public OrderData Data { get; set; } = new OrderData();
    }
}
