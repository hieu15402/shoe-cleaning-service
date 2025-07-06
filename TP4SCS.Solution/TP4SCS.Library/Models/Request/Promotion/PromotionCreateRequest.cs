namespace TP4SCS.Library.Models.Request.Promotion
{
    public class PromotionCreateRequest
    {
        public int ServiceId { get; set; }

        public decimal NewPrice { get; set; }
    }
}
