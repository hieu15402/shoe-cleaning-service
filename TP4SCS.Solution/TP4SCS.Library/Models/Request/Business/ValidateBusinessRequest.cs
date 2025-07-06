using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.Business
{
    public class ValidateBusinessRequest
    {
        [Required]
        [DefaultValue(true)]
        public bool IsApprove { get; set; }
    }
}
