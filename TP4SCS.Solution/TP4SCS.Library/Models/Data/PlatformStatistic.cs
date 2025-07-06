namespace TP4SCS.Library.Models.Data;

public partial class PlatformStatistic
{
    public int Id { get; set; }

    public decimal Value { get; set; }

    public int Date { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }

    public bool IsMonth { get; set; }

    public bool IsYear { get; set; }

    public string Type { get; set; } = null!;
}
