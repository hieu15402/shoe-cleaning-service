namespace TP4SCS.Library.Models.Response.Process
{
    public class ProcessResponse
    {
        public int Id { get; set; }

        public int ServiceId { get; set; }

        public string Process { get; set; } = null!;

        public int ProcessOrder { get; set; }
    }
}
