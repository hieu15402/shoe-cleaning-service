using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.Feedback
{
    public class GetWordBlacklistRequest
    {
        public string? SearchKey { get; set; }

        [Required]
        [DefaultValue(10)]
        [Range(1, int.MaxValue)]
        public int PageSize { get; set; } = 10;

        [Required]
        [DefaultValue(1)]
        [Range(1, int.MaxValue)]
        public int PageNum { get; set; }
    }
}