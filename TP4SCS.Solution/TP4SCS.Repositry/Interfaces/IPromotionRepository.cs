using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;

namespace TP4SCS.Repository.Interfaces
{
    public interface IPromotionRepository
    {
        Task<IEnumerable<Promotion>?> GetPromotionsAsync(string? keyword = null, string? status = null,
            int pageIndex = 1,
            int pageSize = 5,
            OrderByEnum orderBy = OrderByEnum.IdAsc);

        Task<Promotion?> GetPromotionByIdAsync(int id);
        Task<int> GetTotalPromotionsCountAsync(string? keyword = null, string? status = null);
        Task AddPromotionAsync(Promotion promotion);

        Task UpdatePromotionAsync(Promotion promotion);

        Task DeletePromotionAsync(int id);
    }

}
