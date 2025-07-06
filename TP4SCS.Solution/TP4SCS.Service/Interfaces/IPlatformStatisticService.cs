using TP4SCS.Library.Models.Request.PlatformStatistic;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.PlatformStatistic;

namespace TP4SCS.Services.Interfaces
{
    public interface IPlatformStatisticService
    {
        Task<ApiResponse<PlatformStatisticResponse>> GetPlatformStatisticAsync(GetPlatformStatisticRequest getPlatformStatisticRequest);

        Task<ApiResponse<PlatformStatisticResponse>> GetPlatformUserStatisticAsync();

        Task<ApiResponse<PlatformStatisticResponse>> UpdatePlatformStatisticAsync(UpdatePlatformStatisticRequest updatePlatformStatisticRequest);
    }
}
