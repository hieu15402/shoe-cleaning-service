using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.Notification
{
    public enum IdOption
    {
        ACCOUNT,
        BRANCH,
        BUSINESS
    }

    public class GetOrderNotificationRequest
    {
        [Required]
        public int Id { get; set; }

        [DefaultValue(null)]
        public IdOption IdType { get; set; }

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
