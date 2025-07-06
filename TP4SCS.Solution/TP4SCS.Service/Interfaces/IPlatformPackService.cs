using TP4SCS.Library.Models.Request.PlatformPack;
using TP4SCS.Library.Models.Request.SubscriptionPack;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.SubcriptionPack;

namespace TP4SCS.Services.Interfaces
{
    public interface IPlatformPackService
    {
        Task<ApiResponse<IEnumerable<PlatformPackResponse>?>> GetRegisterPacksAsync();

        Task<ApiResponse<IEnumerable<PlatformPackResponse>?>> GetFeaturePacksAsync();

        Task<ApiResponse<PlatformPackResponse?>> GetPackByIdAsync(int id);

        Task<ApiResponse<PlatformPackResponse>> CreateRegisterPackAsync(RegisterPackRequest subscriptionPackRequest);

        Task<ApiResponse<PlatformPackResponse>> UpdateRegisterPackAsync(int id, RegisterPackRequest registerPackRequest);

        Task<ApiResponse<PlatformPackResponse>> UpdateFeaturePackAsync(int id, FeaturePackRequest featurePackRequest);

        Task<ApiResponse<PlatformPackResponse>> DeleteRegisterPackAsync(int id);
    }
}
