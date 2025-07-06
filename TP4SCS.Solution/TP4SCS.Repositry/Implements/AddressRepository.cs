using Microsoft.EntityFrameworkCore;
using TP4SCS.Library.Models.Data;
using TP4SCS.Repository.Interfaces;

namespace TP4SCS.Repository.Implements
{
    public class AddressRepository : GenericRepository<AccountAddress>, IAddressRepository
    {
        public AddressRepository(Tp4scsDevDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task CreateAddressAsync(AccountAddress address)
        {
            await InsertAsync(address);
        }

        public async Task DeletAddressAsync(int id)
        {
            await DeleteAsync(id);
        }

        public async Task<IEnumerable<AccountAddress>?> GetAddressesByAccountIdAsync(int id)
        {
            try
            {
                return await _dbContext.AccountAddresses.Where(a => a.AccountId == id).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<AccountAddress?> GetAddressesByIdAsync(int id)
        {
            try
            {
                return await _dbContext.AccountAddresses.SingleOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<int> GetAddressMaxIdAsync()
        {
            return await _dbContext.AccountAddresses.AsNoTracking().MaxAsync(a => a.Id);
        }

        public async Task<AccountAddress?> GetDefaultAddressesByAccountIdAsync(int id)
        {
            try
            {
                return await _dbContext.AccountAddresses.FirstOrDefaultAsync(a => a.AccountId == id && a.IsDefault == true);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task UpdateAddressAsync(AccountAddress address)
        {
            await UpdateAsync(address);
        }
    }
}
