namespace TP4SCS.Library.Models.Response.Promotion
{
    public class PromotionResponse
    {
        public int Id { get; set; }

        public int ServiceId { get; set; }

        public int SaleOff { get; set; }

        public decimal NewPrice { get; set; }

        public string Status { get; set; } = null!;
    }
}
