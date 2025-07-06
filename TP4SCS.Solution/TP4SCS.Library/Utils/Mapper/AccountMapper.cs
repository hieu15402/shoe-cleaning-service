using Mapster;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Account;
using TP4SCS.Library.Models.Request.Auth;
using TP4SCS.Library.Models.Response.Account;
using TP4SCS.Library.Models.Response.Auth;
using TP4SCS.Library.Utils.StaticClass;

namespace TP4SCS.Library.Utils.Mapper
{
    public class AccountMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<AccountRegisterRequest, Account>()
                .Map(dest => dest.PasswordHash, src => src.Password)
                .Map(dest => dest.ImageUrl, opt => "https://firebasestorage.googleapis.com/v0/b/shoecarehub-4dca3.firebasestorage.app/o/images%2Fe03d0778-890e-4cae-ad06-647a48756770.jpg?alt=media&token=42f7ce37-8693-4e94-bb4d-6615e0cad830")
                .Map(dest => dest.IsVerified, opt => false)
                .Map(dest => dest.IsGoogle, opt => false)
                .Map(dest => dest.RefreshToken, opt => string.Empty)
                .Map(dest => dest.RefreshExpireTime, opt => DateTime.Now)
                .Map(dest => dest.Otp, opt => (int?)null)
                .Map(dest => dest.OtpexpiredTime, opt => DateTime.Now)
                .Map(dest => dest.Fcmtoken, opt => string.Empty)
                .Map(dest => dest.CreatedByOwnerId, opt => (int?)null)
                .Map(dest => dest.Status, opt => StatusConstants.ACTIVE);

            config.NewConfig<Account, UpdateAccountRequest>();

            config.NewConfig<Account, UpdateAccountPasswordRequest>();

            config.NewConfig<Account, AccountResponse>();

            config.NewConfig<Account, EmployeeResponse>();

            config.NewConfig<AuthResponse, Account>();

            config.NewConfig<CreateModeratorRequest, Account>()
                .Map(dest => dest.ImageUrl, opt => "https://firebasestorage.googleapis.com/v0/b/shoecarehub-4dca3.firebasestorage.app/o/images%2Fe03d0778-890e-4cae-ad06-647a48756770.jpg?alt=media&token=42f7ce37-8693-4e94-bb4d-6615e0cad830")
                .Map(dest => dest.IsVerified, opt => true)
                .Map(dest => dest.IsGoogle, opt => false)
                .Map(dest => dest.RefreshToken, opt => string.Empty)
                .Map(dest => dest.RefreshExpireTime, opt => DateTime.Now)
                .Map(dest => dest.Fcmtoken, opt => string.Empty)
                .Map(dest => dest.CreatedByOwnerId, opt => (int?)null)
                .Map(dest => dest.Role, opt => RoleConstants.MODERATOR)
                .Map(dest => dest.Status, opt => "ACTIVE");

            config.NewConfig<CreateEmployeeRequest, Account>()
                .Map(dest => dest.ImageUrl, opt => "https://firebasestorage.googleapis.com/v0/b/shoecarehub-4dca3.firebasestorage.app/o/images%2Fe03d0778-890e-4cae-ad06-647a48756770.jpg?alt=media&token=42f7ce37-8693-4e94-bb4d-6615e0cad830")
                .Map(dest => dest.IsVerified, opt => true)
                .Map(dest => dest.IsGoogle, opt => false)
                .Map(dest => dest.RefreshToken, opt => string.Empty)
                .Map(dest => dest.RefreshExpireTime, opt => DateTime.Now)
                .Map(dest => dest.Fcmtoken, opt => string.Empty)
                .Map(dest => dest.Role, opt => RoleConstants.EMPLOYEE)
                .Map(dest => dest.Status, opt => "ACTIVE");
        }
    }
}
