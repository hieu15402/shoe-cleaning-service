namespace TP4SCS.Library.Models.Response.Location
{
    public class District
    {
        public int ProvinceID { get; set; }
        public int DistrictID { get; set; }
        public string DistrictName { get; set; } = null!;
        public List<string> NameExtension { get; set; } = new List<string>();
    }
}
