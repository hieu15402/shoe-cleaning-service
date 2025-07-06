using System.Text.Json.Serialization;

namespace TP4SCS.Library.Models.Response.Account
{
    public class AccountResponse
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PasswordHash { get; set; } = null;

        public string FullName { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public DateOnly Dob { get; set; }

        public string? ImageUrl { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RefreshToken { get; set; } = null;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? RefreshExpireTime { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Fcmtoken { get; set; } = null;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? CreatedByOwnerId { get; set; }

        public string Role { get; set; } = null!;

        public string Status { get; set; } = null!;
    }
}
