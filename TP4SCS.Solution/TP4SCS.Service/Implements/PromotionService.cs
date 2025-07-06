using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Library.Utils.Utils;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IServiceRepository _serviceRepository;

        public PromotionService(IPromotionRepository promotionRepository, IServiceRepository serviceRepository)
        {
            _promotionRepository = promotionRepository;
            _serviceRepository = serviceRepository;
        }

        public async Task AddPromotionAsync(Promotion promotion)
        {
            if (promotion == null)
            {
                throw new ArgumentNullException(nameof(promotion), "Yêu cầu khuyến mãi không được để trống.");
            }

            if (promotion.NewPrice <= 0)
            {
                throw new ArgumentException("Giá mới phải lớn hơn 0.");
            }

            var service = await _serviceRepository.GetServiceByIdAsync(promotion.ServiceId);
            if (service == null)
            {
                throw new ArgumentException("ID dịch vụ không hợp lệ.");
            }

            if (Util.IsEqual(service.Status, StatusConstants.INACTIVE))
            {
                throw new ArgumentException("Dịch vụ này đã ngưng hoạt động.");
            }

            if (service.Promotion != null)
            {
                throw new InvalidOperationException("Dịch vụ này đã có khuyến mãi. Không thể thêm khuyến mãi mới.");
            }

            promotion.Status = StatusConstants.AVAILABLE;
            promotion.SaleOff = 100 - (int)Math.Round((promotion.NewPrice / service.Price * 100), MidpointRounding.AwayFromZero);
            await _promotionRepository.AddPromotionAsync(promotion);
        }

        public async Task<int> GetTotalPromotionsCountAsync(string? keyword = null, string? status = null)
        {
            return await _promotionRepository.GetTotalPromotionsCountAsync(keyword, status);
        }

        public async Task DeletePromotionAsync(int id)
        {
            var promotion = await _promotionRepository.GetPromotionByIdAsync(id);

            if (promotion == null)
            {
                throw new Exception($"Khuyến mãi với ID {id} không tìm thấy.");
            }

            await _promotionRepository.DeletePromotionAsync(id);
        }

        public async Task<Promotion?> GetPromotionByIdAsync(int id)
        {
            return await _promotionRepository.GetPromotionByIdAsync(id);
        }

        public async Task<IEnumerable<Promotion>?> GetPromotionsAsync(string? keyword = null, string? status = null, int pageIndex = 1, int pageSize = 5, OrderByEnum orderBy = OrderByEnum.IdAsc)
        {
            if (pageIndex < 1)
            {
                throw new ArgumentException("Chỉ số trang phải lớn hơn 0.");
            }

            if (pageSize < 1)
            {
                throw new ArgumentException("Kích thước trang phải lớn hơn 0.");
            }

            return await _promotionRepository.GetPromotionsAsync(keyword, status, pageIndex, pageSize, orderBy);
        }

        public async Task UpdatePromotionAsync(Promotion promotion, int existingPromotionId)
        {
            if (promotion == null)
            {
                throw new ArgumentNullException(nameof(promotion), "Yêu cầu cập nhật khuyến mãi không được để trống.");
            }

            if (promotion.SaleOff < 0 || promotion.SaleOff > 100)
            {
                throw new ArgumentException("Giảm giá phải nằm trong khoảng từ 0 đến 100%.");
            }

            var existingPromotion = await _promotionRepository.GetPromotionByIdAsync(existingPromotionId);
            if (existingPromotion == null)
            {
                throw new KeyNotFoundException($"Khuyến mãi với ID {existingPromotionId} không tìm thấy.");
            }

            var service = await _serviceRepository.GetServiceByIdAsync(existingPromotion.ServiceId);
            if (service == null)
            {
                throw new ArgumentException("ID dịch vụ không hợp lệ.");
            }
            if (Util.IsEqual(service.Status, StatusConstants.INACTIVE))
            {
                throw new ArgumentException("Dịch vụ này đã ngưng hoạt động.");
            }
            existingPromotion.NewPrice = promotion.NewPrice;
            existingPromotion.SaleOff = 100 - (int)Math.Round((promotion.NewPrice / service.Price * 100), MidpointRounding.AwayFromZero);
            existingPromotion.Status = Util.UpperCaseStringStatic(promotion.Status);

            await _promotionRepository.UpdatePromotionAsync(existingPromotion);
        }

        public async Task<bool> IsPromotionActiveAsync(int promotionId)
        {
            var promotion = await _promotionRepository.GetPromotionByIdAsync(promotionId);

            if (promotion == null)
            {
                throw new KeyNotFoundException($"Khuyến mãi với ID {promotionId} không tìm thấy.");
            }

            // Sử dụng Util.IsEqual để kiểm tra trạng thái
            return Util.IsEqual(promotion.Status, StatusConstants.AVAILABLE);
        }
    }
}
