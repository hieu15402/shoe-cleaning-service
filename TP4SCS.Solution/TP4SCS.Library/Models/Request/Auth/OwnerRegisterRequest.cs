using System.ComponentModel.DataAnnotations;
using TP4SCS.Library.Models.Request.BusinessProfile;

namespace TP4SCS.Library.Models.Request.Auth
{
    public class OwnerRegisterRequest
    {
        [Required]
        public AccountRegisterRequest CustomerRegister { get; set; } = new AccountRegisterRequest();

        [Required]
        public CreateBusinessRequest CreateBusiness { get; set; } = new CreateBusinessRequest();
    }
}
