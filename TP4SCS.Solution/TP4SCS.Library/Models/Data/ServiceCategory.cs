namespace TP4SCS.Library.Models.Data;

public partial class ServiceCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
