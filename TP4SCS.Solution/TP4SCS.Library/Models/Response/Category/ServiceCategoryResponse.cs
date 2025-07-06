namespace TP4SCS.Library.Models.Response.Category
{
    public class ServiceCategoryResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public required string Status { get; set; }
    }
}
