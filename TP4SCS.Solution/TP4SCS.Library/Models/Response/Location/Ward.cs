namespace TP4SCS.Library.Models.Response.Location
{
    public class Ward
    {
        public string WardCode { get; set; } = string.Empty;
        public int DistrictID { get; set; }
        public string WardName { get; set; } = string.Empty;
        public List<string> NameExtension { get; set; } = new List<string>();
    }
}