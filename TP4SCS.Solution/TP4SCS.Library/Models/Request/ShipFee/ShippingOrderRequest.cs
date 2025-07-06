namespace TP4SCS.Library.Models.Request.ShipFee
{
    public class ShippingOrderRequest
    {
        public string FromName { get; set; } = string.Empty;
        public string FromPhone { get; set; } = string.Empty;
        public string FromAddress { get; set; } = string.Empty;
        public string FromWardName { get; set; } = string.Empty;
        public string FromDistrictName { get; set; } = string.Empty;
        public string FromProvinceName { get; set; } = string.Empty;

        public string ToName { get; set; } = string.Empty;
        public string ToPhone { get; set; } = string.Empty;
        public string ToAddress { get; set; } = string.Empty;
        public string ToWardCode { get; set; } = string.Empty;
        public int ToDistrictId { get; set; }

        public decimal CODAmount { get; set; }
    }
}
