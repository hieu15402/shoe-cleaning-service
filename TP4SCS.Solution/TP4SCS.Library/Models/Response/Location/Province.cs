namespace TP4SCS.Library.Models.Response.Location
{
    public class Province
    {
        public int ProvinceID { get; set; }
        public string ProvinceName { get; set; } = null!;
        public List<string> NameExtension { get; set; } = new List<string>();

    }
}
