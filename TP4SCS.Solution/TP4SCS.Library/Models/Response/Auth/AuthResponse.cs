using System.ComponentModel;
using System.Text.Json.Serialization;

namespace TP4SCS.Library.Models.Response.Auth
{
    public class AuthResponse
    {
        public int Id { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [DefaultValue(null)]
        public int? BusinessId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [DefaultValue(null)]
        public bool? IsIndividual { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [DefaultValue(null)]
        public bool? IsMaterialSupported { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [DefaultValue(null)]
        public bool? IsLimitServiceNum { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [DefaultValue(null)]
        public int? BranchId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public DateOnly Dob { get; set; }

        public string? ImageUrl { get; set; } = string.Empty;

        public string? RefreshToken { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Fcmtoken { get; set; }

        public string Role { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;

        public int ExpiresIn { get; set; }

    }
}
