namespace TP4SCS.Library.Models.Request.ShipFee
{
    public class GetShipFeeRequest
    {
        public int FromDistricId { get; set; }

        public string FromWardCode { get; set; } = string.Empty;

        public int ToDistricId { get; set; }

        public string ToWardCode { get; set; } = string.Empty;
    }
}
