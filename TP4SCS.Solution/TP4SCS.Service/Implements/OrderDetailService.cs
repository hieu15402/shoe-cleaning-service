using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Library.Utils.Utils;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IServiceService _serviceService;
        private readonly IMaterialRepository _materialRepository;
        private readonly IAssetUrlService _assetUrlService;
        private readonly IBranchRepository _branchRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IOrderRepository _orderRepository;

        public OrderDetailService(IOrderDetailRepository orderDetailRepository, IServiceService serviceService,
            IMaterialRepository materialRepository, IAssetUrlService assetUrlService, IBranchRepository branchRepository,
            IBusinessRepository businessRepository, IOrderRepository orderRepository)
        {
            _orderDetailRepository = orderDetailRepository;
            _serviceService = serviceService;
            _materialRepository = materialRepository;
            _assetUrlService = assetUrlService;
            _branchRepository = branchRepository;
            _businessRepository = businessRepository;
            _orderRepository = orderRepository;
        }

        //public async Task AddOrderDetailsAsync(List<OrderDetail> orderDetails)
        //{
        //    foreach (var orderDetail in orderDetails)
        //    {

        //        // Kiểm tra Material
        //        if (orderDetail.MaterialId.HasValue)
        //        {
        //            var material = await _materialRepository.GetMaterialByIdAsync(orderDetail.MaterialId.Value);
        //            if (material == null || material.Status != StatusConstants.AVAILABLE)
        //            {
        //                throw new InvalidOperationException("Vật liệu được chỉ định không có sẵn hoặc không hoạt động.");
        //            }
        //        }
        //        // Kiểm tra Service
        //        var service = await _serviceService.GetServiceByIdAsync((int)orderDetail.ServiceId!);
        //        if (service == null || service.Status != StatusConstants.AVAILABLE)
        //        {
        //            throw new InvalidOperationException("Dịch vụ được chỉ định không có sẵn hoặc không hoạt động.");
        //        }
        //        // Kiểm tra quantity
        //        //if (orderDetail.Quantity <= 0)
        //        //{
        //        //    throw new InvalidOperationException("Số lượng phải lớn hơn 0.");
        //        //}

        //        // Kiểm tra price
        //        if (orderDetail.Price <= 0)
        //        {
        //            throw new InvalidOperationException("Giá phải lớn hơn 0.");
        //        }
        //    }

        //    await _orderDetailRepository.AddOrderDetailsAsync(orderDetails);
        //}

        public async Task AddOrderDetailAsync(int orderId, int branchId, int serviceId, List<int> materialIds)
        {
            OrderDetail orderDetail = new OrderDetail();
            orderDetail.OrderId = orderId;
            orderDetail.BranchId = branchId;
            orderDetail.ServiceId = serviceId;
            // Kiểm tra Material
            if (materialIds != null && materialIds.Any())
            {
                foreach (int materialId in materialIds)
                {
                    var material = await _materialRepository.GetMaterialByIdAsync(materialId);
                    if (material == null || material.Status != StatusConstants.AVAILABLE)
                    {
                        throw new InvalidOperationException("Vật liệu được chỉ định không có sẵn hoặc không hoạt động.");
                    }
                    orderDetail.Price += material.Price;
                }
                orderDetail.MaterialIds = Util.ConvertListToString(materialIds);
            }
            var service = await _serviceService.GetServiceByIdAsync(serviceId);
            if (service == null || service.Status != StatusConstants.AVAILABLE)
            {
                throw new InvalidOperationException("Dịch vụ được chỉ định không có sẵn hoặc không hoạt động.");
            }
            orderDetail.Price += await _serviceService.GetServiceFinalPriceAsync(orderDetail.ServiceId.Value);

            await _orderDetailRepository.AddOrderDetailAsync(orderDetail);
        }

        public async Task DeleteOrderDetailAsync(int id)
        {
            await _orderDetailRepository.DeleteOrderDetailAsync(id);
        }

        public async Task<OrderDetail?> GetOrderDetailByIdAsync(int id)
        {
            return await _orderDetailRepository.GetOrderDetailByIdAsync(id);
        }

        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            return await _orderDetailRepository.GetOrderDetailsByOrderIdAsync(orderId);
        }

        public async Task UpdateOrderDetailAsync(int existingOrderDetailId, OrderDetail orderDetail)
        {
            var existingOrderDetail = await _orderDetailRepository.GetOrderDetailByIdAsync(existingOrderDetailId);
            if (existingOrderDetail == null)
            {
                throw new InvalidOperationException($"Không tìm thấy Order Detail với id: {existingOrderDetailId}");
            }
            if (orderDetail.ProcessState != null)
            {
                existingOrderDetail.ProcessState = orderDetail.ProcessState;
            }
            if (orderDetail.AssetUrls != null)
            {
                var existingAssetUrls = existingOrderDetail.AssetUrls.ToList();
                var newAssetUrls = orderDetail.AssetUrls;

                var newUrls = newAssetUrls.Select(a => a.Url).ToList();

                var urlsToRemove = existingOrderDetail.AssetUrls.Where(a => !newUrls.Contains(a.Url)).ToList();
                if (urlsToRemove.Any())
                {
                    foreach (var assetUrl in urlsToRemove)
                    {
                        await _assetUrlService.DeleteAssetUrlAsync(assetUrl.Id);
                        existingOrderDetail.AssetUrls.Remove(assetUrl);
                    }
                }

                var urlsToAdd = newAssetUrls.Where(a => !existingAssetUrls.Any(e => e.Url == a.Url)).ToList();
                if (urlsToAdd.Any())
                {
                    foreach (var newAsset in urlsToAdd)
                    {
                        var newAssetUrl = new AssetUrl
                        {
                            Url = newAsset.Url,
                            //IsImage = newAsset.IsImage,
                            Type = newAsset.Type
                        };
                        existingOrderDetail.AssetUrls.Add(newAssetUrl);
                    }
                }
            }
            await _orderDetailRepository.UpdateOrderDetailAsync(existingOrderDetail);
        }

    }
}
