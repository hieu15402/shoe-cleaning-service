using TP4SCS.Library.Models.Data;

namespace TP4SCS.Repository.Interfaces
{
    public interface IAddressRepository : IGenericRepository<AccountAddress>
    {
        Task<IEnumerable<AccountAddress>?> GetAddressesByAccountIdAsync(int id);

        Task<AccountAddress?> GetAddressesByIdAsync(int id);

        Task<AccountAddress?> GetDefaultAddressesByAccountIdAsync(int id);

        Task CreateAddressAsync(AccountAddress address);

        Task UpdateAddressAsync(AccountAddress address);

        Task DeletAddressAsync(int id);

        Task<int> GetAddressMaxIdAsync();
    }
}
