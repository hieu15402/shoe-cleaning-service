namespace TP4SCS.Library.Models.Data;

public partial class Account
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsGoogle { get; set; }

    public bool IsVerified { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshExpireTime { get; set; }

    public int? Otp { get; set; }

    public DateTime? OtpexpiredTime { get; set; }

    public string? Fcmtoken { get; set; }

    public int? CreatedByOwnerId { get; set; }

    public string Role { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<AccountAddress> AccountAddresses { get; set; } = new List<AccountAddress>();

    public virtual BusinessProfile? BusinessProfile { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<SupportTicket> SupportTicketModerators { get; set; } = new List<SupportTicket>();

    public virtual ICollection<SupportTicket> SupportTicketUsers { get; set; } = new List<SupportTicket>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
