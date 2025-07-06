namespace TP4SCS.Library.Models.Response.ShipOrder
{
    public class OrderData
    {
        public string Status { get; set; } = string.Empty;
        public List<OrderLog> Logs { get; set; } = new List<OrderLog>();
    }
}
