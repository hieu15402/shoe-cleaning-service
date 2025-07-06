using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;

namespace TP4SCS.Services.Interfaces
{
    public interface IPromotionService
    {
        Task<Promotion?> GetPromotionByIdAsync(int id);
        Task<IEnumerable<Promotion>?> GetPromotionsAsync(
            string? keyword = null,
            string? status = null,
            int pageIndex = 1,
            int pageSize = 5,
            OrderByEnum orderBy = OrderByEnum.IdAsc);
        Task AddPromotionAsync(Promotion promotion);
        Task<int> GetTotalPromotionsCountAsync(string? keyword = null, string? status = null);
        Task UpdatePromotionAsync(Promotion promotion, int existingPromotionId);
        Task DeletePromotionAsync(int id);
        Task<bool> IsPromotionActiveAsync(int promotionId);
    }

}
