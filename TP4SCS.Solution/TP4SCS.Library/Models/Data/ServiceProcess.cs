namespace TP4SCS.Library.Models.Data;

public partial class ServiceProcess
{
    public int Id { get; set; }

    public int ServiceId { get; set; }

    public string Process { get; set; } = null!;

    public int ProcessOrder { get; set; }

    public virtual Service Service { get; set; } = null!;
}
