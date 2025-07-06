using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.Category
{
    public class ServiceCategoryRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}
