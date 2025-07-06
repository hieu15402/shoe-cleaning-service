using System.ComponentModel.DataAnnotations;
using TP4SCS.Library.Models.Request.General;

namespace TP4SCS.Library.Models.Request.Account
{
    public class UpdateStatusRequest
    {
        [Required]
        public AccountStatus Status { get; set; }
    }
}
