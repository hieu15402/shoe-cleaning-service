namespace TP4SCS.Library.Models.Request.Promotion
{
    public class PromotionUpdateRequest
    {
        public decimal NewPrice { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
