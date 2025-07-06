using TP4SCS.Library.Models.Request.Auth;
using TP4SCS.Library.Models.Response.Auth;
using TP4SCS.Library.Models.Response.General;

namespace TP4SCS.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest loginRequest);

        Task<ApiResponse<AuthResponse>> LoginOTPAsync(LoginOTPRequest loginOTPRequest);
        Task<ApiResponse<AuthResponse>> LoginGoogleAsync(string email);
        Task<ApiResponse<AuthResponse>> GetUserByToken(string token);
        Task<ApiResponse<AuthResponse>> CustomerRegisterAsync(AccountRegisterRequest customerRegisterRequest);

        Task<ApiResponse<AuthResponse>> OwnerRegisterAsync(HttpClient httpClient, OwnerRegisterRequest ownerRegisterRequest);

        Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshToken refeshToken);

        Task<ApiResponse<AuthResponse>> SendOTPAsync(string email);

        Task<ApiResponse<AuthResponse>> SendVerificationEmailAsync(string email);

        Task<ApiResponse<AuthResponse>> ResendVerificationEmailAsync(int id);

        Task<ApiResponse<AuthResponse>> VerifyEmailAsync(VerifyEmailRequest verifyEmailRequest);

        Task<ApiResponse<AuthResponse>> ResetPasswordAsync(ResetPasswordQuery resetPasswordQuery, ResetPasswordRequest resetPasswordRequest);

        Task<ApiResponse<AuthResponse>> RequestResetPasswordAsync(string email);

        Task<ApiResponse<AuthResponse>> SendAccountInfoEmail(string email, string password);
    }
}