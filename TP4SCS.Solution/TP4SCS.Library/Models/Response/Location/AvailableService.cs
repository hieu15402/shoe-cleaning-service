namespace TP4SCS.Library.Models.Response.Location
{
    public class AvailableService
    {
        public int ServiceID { get; set; }
        public string ShortName { get; set; } = null!;
        public int ServiceTypeID { get; set; }
    }
}