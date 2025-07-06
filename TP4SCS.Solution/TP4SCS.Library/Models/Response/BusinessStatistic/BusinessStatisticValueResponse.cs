using System.ComponentModel;
using System.Text.Json.Serialization;

namespace TP4SCS.Library.Models.Response.BusinessStatistic
{
    public class BusinessStatisticValueResponse
    {
        public decimal Value { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [DefaultValue(null)]
        public int? Date { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public bool ShouldSerializeDate()
        {
            return Date != 0;
        }
    }
}
