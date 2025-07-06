namespace TP4SCS.Library.Models.Response.Category
{
    public class TicketCategoryResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Priority { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
