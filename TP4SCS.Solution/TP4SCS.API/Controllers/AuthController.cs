using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using TP4SCS.Library.Models.Request.Auth;
using TP4SCS.Library.Models.Response.Auth;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IAccountService _accountService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public AuthController(IAuthService authService, IAccountService accountService, HttpClient httpClient, IConfiguration config)
        {
            _authService = authService;
            _accountService = accountService;
            _httpClient = httpClient;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest)
        {
            var result = await _authService.LoginAsync(loginRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpGet("login-by-google")]
        public IActionResult GoogleLogin()
        {
            var clientId = _config["GoogleAuthSettings:ClientId"];
            var redirectUri = _config["GoogleAuthSettings:RedirectUri"];
            var googleLoginUrl = $"https://accounts.google.com/o/oauth2/v2/auth?response_type=code&client_id={clientId}&redirect_uri={redirectUri}&scope=openid%20email%20profile";

            if (!Uri.TryCreate(googleLoginUrl, UriKind.Absolute, out Uri? link))
            {
                return BadRequest("Định Dạng URL Không Hợp Lệ!");
            }

            return Ok(link.ToString());
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback([FromQuery] string code)
        {
            var httpClient = new HttpClient();

            var tokenResponse = await httpClient.PostAsync("https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                { "code", code },
                { "client_id", _config["GoogleAuthSettings:ClientId"]! },
                { "client_secret", _config["GoogleAuthSettings:ClientSecret"]! },
                { "redirect_uri", _config["GoogleAuthSettings:RedirectUri"]! },
                { "grant_type", "authorization_code" }
                }));

            var tokenResponseJson = await tokenResponse.Content.ReadAsStringAsync();
            var tokenData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(tokenResponseJson);
            var idToken = tokenData!.id_token.ToString();

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);

            ApiResponse<AuthResponse> result = await _authService.LoginGoogleAsync(payload.Email);
            var url = "https://www.shoecarehub.xyz/auth?token=";
            if (result.Data != null)
            {
                return Redirect("https://www.shoecarehub.xyz/auth?token="+ result.Data.Token);
            }
            return Redirect("https://www.shoecarehub.xyz/register");
        }
        [HttpGet("get-by-token")]
        public async Task<IActionResult> GetUserByToken([FromQuery] string token)
        {
            var result = await _authService.GetUserByToken(token);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpPost("login-by-otp")]
        public async Task<IActionResult> LoginOTPAsync([FromBody] LoginOTPRequest loginOTPRequest)
        {
            var result = await _authService.LoginOTPAsync(loginOTPRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshToken refreshToken)
        {
            var result = await _authService.RefreshTokenAsync(refreshToken);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpPost("customer-register")]
        public async Task<IActionResult> CreateCustomerAccountAsync([FromBody] AccountRegisterRequest createAccountRequest)
        {
            var result = await _authService.CustomerRegisterAsync(createAccountRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return StatusCode(201, result.Data);
        }

        [HttpPost("owner-register")]
        public async Task<IActionResult> CreateOwnerAccountAsync([FromBody] OwnerRegisterRequest ownerRegisterRequest)
        {
            var result = await _authService.OwnerRegisterAsync(_httpClient, ownerRegisterRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return StatusCode(201, result.Data);
        }

        [HttpPost("{id}/resend-verification-email")]
        public async Task<IActionResult> ResendVerificationEmailAsync([FromRoute] int id)
        {
            var result = await _authService.ResendVerificationEmailAsync(id);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmailAsync([FromQuery] VerifyEmailRequest verifyEmailRequest)
        {
            var result = await _authService.VerifyEmailAsync(verifyEmailRequest);

            if (result.StatusCode != 200)
            {
                return Redirect($"https://www.shoecarehub.xyz/confirm-fail?{verifyEmailRequest.AccountId}");
            }

            return Redirect("https://www.shoecarehub.xyz/confirm-success");
        }

        [HttpPost("request-reset-password")]
        public async Task<IActionResult> RequestResetPasswordAsync([FromBody] EmailRequest emailRequest)
        {
            var result = await _authService.RequestResetPasswordAsync(emailRequest.Email);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpPost("request-otp-code")]
        public async Task<IActionResult> RequestOTPCodeAsync([FromBody] EmailRequest emailRequest)
        {
            var result = await _authService.SendOTPAsync(emailRequest.Email);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync([FromQuery] ResetPasswordQuery resetPasswordQuery, [FromBody] ResetPasswordRequest resetPasswordRequest)
        {
            var result = await _authService.ResetPasswordAsync(resetPasswordQuery, resetPasswordRequest);

            var url = "https://www.shoecarehub.xyz/reset-success";

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? link))
            {
                return BadRequest("Định Dạng URL Không Hợp Lệ!");
            }

            return Ok(link.ToString());
        }
    }
}
