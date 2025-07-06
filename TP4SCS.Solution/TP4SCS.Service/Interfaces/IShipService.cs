using TP4SCS.Library.Models.Request.ShipFee;
using TP4SCS.Library.Models.Response.Location;

namespace TP4SCS.Services.Interfaces
{
    public interface IShipService
    {
        Task<List<AvailableService>?> GetAvailableServicesAsync(HttpClient httpClient, int fromDistrict, int toDistrict);

        Task<List<District>?> GetDistrictsByProvinceIdAsync(HttpClient httpClient, int provinceId);

        Task<string?> GetDistrictNamByIdAsync(HttpClient httpClient, int id);

        Task<List<Province>?> GetProvincesAsync(HttpClient httpClient);

        Task<string?> GetProvinceNameByIdAsync(HttpClient httpClient, int id);

        Task<decimal> GetShippingFeeAsync(HttpClient httpClient, GetShipFeeRequest getShipFeeRequest);

        Task<List<Ward>?> GetWardsByDistrictIdAsync(HttpClient httpClient, int id);

        Task<string?> GetWardNameByWardCodeAsync(HttpClient httpClient, int districtId, string code);
        Task<string?> CreateShippingOrderAsync(HttpClient httpClient, ShippingOrderRequest request);
        Task<(string? Status, List<(string Status, string UpdatedDate)> Logs)> GetOrderStatusAsync(HttpClient httpClient, string orderCode);
    }
}
