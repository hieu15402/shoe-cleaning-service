namespace TP4SCS.Library.Models.Response.BusinessStatistic
{
    public class BusinessStatisticResponse
    {
        public int BusinessId { get; set; }

        public string Type { get; set; } = string.Empty;

        public List<BusinessStatisticValueResponse> Value { get; set; } = new List<BusinessStatisticValueResponse>();
    }
}
