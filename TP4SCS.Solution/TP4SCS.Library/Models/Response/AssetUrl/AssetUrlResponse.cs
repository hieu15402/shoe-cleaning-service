namespace TP4SCS.Library.Models.Response.AssetUrl
{
    public class AssetUrlResponse
    {
        public int Id { get; set; }

        public int? ServiceId { get; set; }

        public int? Feedbackid { get; set; }

        public string Url { get; set; } = null!;

        public bool IsImage { get; set; }

        public string Type { get; set; } = null!;
    }
}
