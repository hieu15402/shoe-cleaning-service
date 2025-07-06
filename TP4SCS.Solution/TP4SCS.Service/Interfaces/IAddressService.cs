using TP4SCS.Library.Models.Request.Address;
using TP4SCS.Library.Models.Response.Address;
using TP4SCS.Library.Models.Response.General;

namespace TP4SCS.Services.Interfaces
{
    public interface IAddressService
    {
        Task<ApiResponse<IEnumerable<AddressResponse>?>> GetAddressesByAccountIdAsync(int id);

        Task<ApiResponse<AddressResponse?>> GetAddressesByIdAsync(int id);

        Task<int> GetAddressMaxIdAsync();

        Task<ApiResponse<AddressResponse>> CreateAddressAsync(HttpClient httpClient, CreateAddressRequest createAddressRequest);

        Task<ApiResponse<AddressResponse>> UpdateAddressAsync(int id, HttpClient httpClient, UpdateAddressRequest updateAddressRequest);

        Task<ApiResponse<AddressResponse>> UpdateAddressDefaultAsync(int id);

        Task<ApiResponse<AddressResponse>> DeleteAddressAsync(int id);
    }
}
