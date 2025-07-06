namespace TP4SCS.Library.Models.Data;

public partial class Leaderboard
{
    public int Id { get; set; }

    public string BusinessIds { get; set; } = null!;

    public int Month { get; set; }

    public int Year { get; set; }

    public bool IsMonth { get; set; }

    public bool IsYear { get; set; }
}
