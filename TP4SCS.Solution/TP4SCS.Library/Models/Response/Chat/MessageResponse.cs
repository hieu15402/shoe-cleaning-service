using System.ComponentModel;
using System.Text.Json.Serialization;

namespace TP4SCS.Library.Models.Response.Chat
{
    public class MessageResponse
    {
        public string Id { get; set; } = string.Empty;

        public string RoomId { get; set; } = string.Empty;

        public int SenderId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Content { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? ImageUrls { get; set; }

        public bool IsImage { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DefaultValue(false)]
        public bool IsOrder { get; set; } = false;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DefaultValue(false)]
        public bool IsOwner { get; set; } = false;

        public DateTime Timestamp { get; set; }
    }
}
