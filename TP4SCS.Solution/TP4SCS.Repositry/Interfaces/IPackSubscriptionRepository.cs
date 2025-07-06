using TP4SCS.Library.Models.Data;

namespace TP4SCS.Repository.Interfaces
{
    public interface IPackSubscriptionRepository : IGenericRepository<PackSubscription>
    {
        Task<bool> CheckRegisteredPackOfBusinessAsync(int id, string feature);

        Task CreatePackSubscriptionAsync(PackSubscription packSubscription);
    }
}
