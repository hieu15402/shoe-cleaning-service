using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.Business
{
    public class UpdateBusinessStatusRequest
    {
        [Required]
        public BusinessStatus Status { get; set; }
    }
}
