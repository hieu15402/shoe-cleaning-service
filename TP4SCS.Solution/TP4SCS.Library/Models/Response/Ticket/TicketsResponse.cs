using System.Text.Json.Serialization;

namespace TP4SCS.Library.Models.Response.Ticket
{
    public class TicketsResponse
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ModeratorId { get; set; } = null;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ModeratorName { get; set; } = null;

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int Priority { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? OrderId { get; set; } = null;

        public string Title { get; set; } = string.Empty;

        public DateTime CreateTime { get; set; }

        public bool IsSeen { get; set; }

        public bool IsOwnerNoti { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
