using System.ComponentModel;
using System.Text.Json.Serialization;

namespace TP4SCS.Library.Models.Response.SubcriptionPack
{
    public class PlatformPackResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int Period { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DefaultValue(0)]
        public decimal? Price { get; set; }

        public string? Feature { get; set; }

        public string Type { get; set; } = string.Empty;
    }
}
