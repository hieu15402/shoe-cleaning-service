using MapsterMapper;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Library.Models.Request.Notification;
using TP4SCS.Library.Models.Request.Order;
using TP4SCS.Library.Models.Request.ShipFee;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Library.Utils.Utils;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IBusinessService _businessService;
        private readonly IAddressRepository _addressRepository;
        private readonly IMaterialService _materialService;
        private readonly IAccountRepository _accountRepository;
        private readonly IShipService _shipService;
        private readonly IOrderNotificationRepository _orderNotificationRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService emailService;

        public OrderService(
            IOrderRepository orderRepository,
            IServiceRepository serviceRepository,
            IMaterialService materialService,
            IAddressRepository addressRepository,
            IBranchRepository branchRepository,
            IAccountRepository accountRepository,
            IShipService shipService,
            IOrderNotificationRepository orderNotificationRepository,
            IMapper mapper,
            IBusinessRepository businessRepository,
            IBusinessService businessService,
            IEmailService emailService)
        {
            _orderRepository = orderRepository;
            _serviceRepository = serviceRepository;
            _materialService = materialService;
            _addressRepository = addressRepository;
            _branchRepository = branchRepository;
            _accountRepository = accountRepository;
            _shipService = shipService;
            _orderNotificationRepository = orderNotificationRepository;
            _mapper = mapper;
            _businessRepository = businessRepository;
            _businessService = businessService;
            this.emailService = emailService;
        }

        public async Task<IEnumerable<Order>?> GetOrdersAsync(string? status = null,
            int? pageIndex = null,
            int? pageSize = null,
            OrderedOrderByEnum orderBy = OrderedOrderByEnum.CreateDateAsc)
        {
            return await _orderRepository.GetOrdersAsync(status, pageIndex, pageSize, orderBy);
        }

        public async Task<Order?> GetOrderByOrderId(int orderId)
        {
            return await _orderRepository.GetOrderByIdAsync(orderId);
        }
        public async Task<Order?> GetOrderByShipCode(string shipCode)
        {
            return await _orderRepository.GetOrderByCodeAsync(shipCode);
        }

        public async Task<IEnumerable<Order>?> GetOrdersByAccountIdAsync(
            int accountId,
            string? status = null,
            OrderedOrderByEnum orderBy = OrderedOrderByEnum.CreateDateAsc)
        {
            var orders = await GetOrdersAsync(status, null, null, orderBy);
            if (orders == null)
            {
                return Enumerable.Empty<Order>();
            }
            // Lọc đơn hàng theo AccountId
            return orders.Where(o => o.AccountId == accountId);
        }

        public async Task<IEnumerable<Order>?> GetOrdersByBranchIdAsync(
            int branchId,
            string? status = null,
            OrderedOrderByEnum orderBy = OrderedOrderByEnum.CreateDateAsc)
        {
            var orders = await GetOrdersAsync(status, null, null, orderBy);
            if (orders == null)
            {
                return Enumerable.Empty<Order>();
            }
            return orders.Where(o => o.OrderDetails.Any(od => od.BranchId == branchId));
        }
        public async Task<IEnumerable<Order>?> GetOrdersByBusinessIdAsync(
            int businessId,
            string? status = null,
            OrderedOrderByEnum orderBy = OrderedOrderByEnum.CreateDateAsc)
        {
            var orders = await GetOrdersAsync(status, null, null, orderBy);
            if (orders == null)
            {
                return Enumerable.Empty<Order>();
            }
            return orders.Where(o => o.OrderDetails.Any(od => od.Branch.BusinessId == businessId));
        }

        //public async Task ApprovedOrder(int orderId)
        //{
        //    var order = await _orderRepository.GetOrderByIdAsync(orderId);

        //    if (order == null)
        //    {
        //        throw new InvalidOperationException($"Đơn hàng với ID {orderId} không tồn tại.");
        //    }

        //    if (!Util.IsEqual(order.Status, StatusConstants.PENDING))
        //    {
        //        throw new InvalidOperationException($"Đơn hàng với ID {orderId} không ở trạng thái chờ duyệt.");
        //    }

        //    await UpdateOrderStatusAsync(orderId, StatusConstants.APPROVED);
        //}
        public async Task CreateShipOrder(HttpClient httpClient, int orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                throw new InvalidOperationException($"Đơn hàng với ID {orderId} không tồn tại.");
            }
            var address = await _addressRepository.GetByIDAsync(order.AddressId!);
            var branch = await _branchRepository.GetByIDAsync(order.OrderDetails.FirstOrDefault()!.BranchId);
            var account = await _accountRepository.GetByIDAsync(order.AccountId);
            if (branch == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy chi nhánh với ID: {order.OrderDetails.FirstOrDefault()!.BranchId}");
            }
            if (address == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy địa chỉ của tài khoản với ID: {order.AddressId!}");
            }
            if (account == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy tài khoản với ID: {order.AccountId}");
            }
            ShippingOrderRequest shippingOrder = new ShippingOrderRequest
            {
                FromName = branch.Name,
                FromPhone = "0901515646",
                FromAddress = branch.Address,
                FromDistrictName = branch.District,
                FromProvinceName = branch.Province,
                FromWardName = branch.Ward,

                ToAddress = address.Address,
                ToPhone = "0907514561",
                ToDistrictId = address.DistrictId,
                ToName = account.FullName,
                ToWardCode = address.WardCode,

                CODAmount = Convert.ToInt32(order.OrderPrice)
            };
            var code = await _shipService.CreateShippingOrderAsync(httpClient, shippingOrder);
            if (code == null)
            {
                throw new KeyNotFoundException("Tạo mã vận đơn thất bại.");
            }
            order.ShippingCode = code;
            await _orderRepository.UpdateOrderAsync(order);
        }
        public async Task UpdateOrderStatusAsync(HttpClient httpClient, int existingOrderedId, string newStatus)
        {
            if (!Util.IsValidOrderStatus(newStatus))
            {
                throw new ArgumentException("Order status không hợp lệ");
            }

            var status = newStatus.Trim().ToUpperInvariant();

            var order = await _orderRepository.GetOrderByIdAsync(existingOrderedId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy đơn hàng với ID: {existingOrderedId}");
            }
            if (Util.IsEqual(status, StatusConstants.CANCELED) && !Util.IsEqual(order.Status, StatusConstants.PENDING))
            {
                return;
            }
            var (branchId, businessId) = await _orderRepository.GetBranchIdAndBusinessIdByOrderId(existingOrderedId);
            var branch = await _branchRepository.GetBranchByIdAsync(branchId);
            var business = await _businessRepository.GetBusinessProfileByIdAsync(businessId);
            if (branch == null || business == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy branch và business của đơn hàng");
            }
            var notification = new CreateOrderNotificationRequest();
            notification.OrderId = order.Id;

            var newNoti = _mapper.Map<OrderNotification>(notification);

            if (Util.IsEqual(status, StatusConstants.CANCELED) && Util.IsEqual(order.Status, StatusConstants.PENDING))
            {
                var orderDetails = order.OrderDetails;
                foreach (var od in orderDetails)
                {
                    if (od.ServiceId.HasValue)
                    {
                        var service = await _serviceRepository.GetServiceByIdAsync(od.ServiceId.Value);
                        service!.OrderedNum--;
                        if (service!.OrderedNum < 0) service!.OrderedNum = 0;
                        await _serviceRepository.UpdateServiceAsync(service);
                    }
                    if (!string.IsNullOrEmpty(od.MaterialIds))
                    {
                        var materialIds = Util.ConvertStringToList(od.MaterialIds);
                        foreach (var materialId in materialIds)
                        {
                            var material = await _materialService.GetMaterialByIdAsync(materialId);
                            material!.BranchMaterials.SingleOrDefault(bm => bm.BranchId == od.BranchId)!.Storage++;
                            await _materialService.UpdateMaterialAsync(material);
                        }
                    }
                }

                business.PendingAmount--;
                business.CanceledAmount++;
                branch.PendingAmount--;
                branch.CanceledAmount++;

                await _businessRepository.UpdateBusinessProfileAsync(business);
                await _branchRepository.UpdateBranchAsync(branch);

                newNoti.Content = "Bạn Có Một Đơn Hàng Bị Huỷ!";
                newNoti.IsProviderNoti = true;
                await _orderNotificationRepository.CreateOrderNotificationAsync(newNoti);
            }
            else if (Util.IsEqual(status, StatusConstants.APPROVED) && Util.IsEqual(order.Status, StatusConstants.PENDING))
            {
                business.PendingAmount--;
                branch.PendingAmount--;
                if (branch.PendingAmount < 0) branch.PendingAmount = 0;
                if (business.PendingAmount < 0) business.PendingAmount = 0;
                await _businessRepository.UpdateBusinessProfileAsync(business);
                await _branchRepository.UpdateBranchAsync(branch);

                newNoti.Content = "Đơn Hàng Của Bạn Đã Được Xác Nhận!";
                newNoti.IsProviderNoti = false;
                await _orderNotificationRepository.CreateOrderNotificationAsync(newNoti);
                await emailService.SendEmailAsync(order.Account.Email, "ShoeCareHub Đơn Hàng", "Đơn Hàng Của Bạn Đã Được Xác Nhận");
            }
            else if (Util.IsEqual(status, StatusConstants.APPROVED) && Util.IsEqual(order.Status, StatusConstants.FINISHED))
            {
                business.FinishedAmount--;
                branch.FinishedAmount--;
                business.TotalOrder--;
                if (business.FinishedAmount < 0)
                {
                    business.FinishedAmount = 0;
                }
                if (branch.FinishedAmount < 0)
                {
                    branch.FinishedAmount = 0;
                }
                if (business.TotalOrder < 0)
                {
                    business.TotalOrder = 0;
                }
                await _businessRepository.UpdateBusinessProfileAsync(business);
                await _branchRepository.UpdateBranchAsync(branch);

                var orderDetails = order.OrderDetails;
                foreach (var od in orderDetails)
                {
                    od.ProcessState = null;
                }
            }
            else if (Util.IsEqual(status, StatusConstants.FINISHED))
            {
                business.FinishedAmount++;
                branch.FinishedAmount++;
                business.TotalOrder++;
                await _businessRepository.UpdateBusinessProfileAsync(business);
                await _branchRepository.UpdateBranchAsync(branch);

                if (order.IsComplained == true)
                {
                    await emailService.SendEmailAsync(order.Account.Email, "ShoeCareHub Đơn Hàng", "Đơn Hàng Của Bạn Đã Tái Xử Lý Thành Công. Nếu Không Có Phản Hồi Mới Đơn Khiếu Nại Sẽ Tự Động Đóng Sau 24h");
                }
            }
            else if (Util.IsEqual(status, StatusConstants.PROCESSING))
            {
                business.ProcessingAmount++;
                branch.ProcessingAmount++;

                await _businessRepository.UpdateBusinessProfileAsync(business);
                await _branchRepository.UpdateBranchAsync(branch);
            }
            else if (Util.IsEqual(status, StatusConstants.STORAGE))
            {
                business.ProcessingAmount--;
                branch.ProcessingAmount--;
                if (branch.PendingAmount < 0) branch.PendingAmount = 0;
                if (business.PendingAmount < 0) business.PendingAmount = 0;
                await _businessRepository.UpdateBusinessProfileAsync(business);
                await _branchRepository.UpdateBranchAsync(branch);
            }
            else if (Util.IsEqual(status, StatusConstants.SHIPPING))
            {
                await CreateShipOrder(httpClient, existingOrderedId);

                newNoti.Content = "Đơn Hàng Của Bạn Đã Được Vận Chuyển!";
                newNoti.IsProviderNoti = false;
                await _orderNotificationRepository.CreateOrderNotificationAsync(newNoti);
            }
            else if (Util.IsEqual(status, StatusConstants.STORAGE))
            {
                newNoti.Content = "Đơn Hàng Của Bạn Đã Hoàn Tất Xử Lý!";
                newNoti.IsProviderNoti = false;
                await _orderNotificationRepository.CreateOrderNotificationAsync(newNoti);
            }
            else if (Util.IsEqual(status, StatusConstants.ABANDONED))
            {
                newNoti.Content = "Bạn Có Một Đơn Hàng Quá Hạn Lưu Trữ!";
                newNoti.IsProviderNoti = false;
                await _orderNotificationRepository.CreateOrderNotificationAsync(newNoti);
            }
            else if (Util.IsEqual(status, StatusConstants.DELIVERED))
            {
                newNoti.Content = "Đơn Hàng Của Bạn Đã Giao Thành Công!";
                newNoti.IsProviderNoti = false;
                await _orderNotificationRepository.CreateOrderNotificationAsync(newNoti);
            }

            order.Status = status;
            await _orderRepository.UpdateOrderAsync(order);
        }

        public async Task UpdateOrderAsync(int existingOrderId, UpdateOrderRequest request)
        {
            var existingOrder = await _orderRepository.GetOrderByIdAsync(existingOrderId);

            if (existingOrder == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy đơn hàng với ID: {existingOrderId}");
            }

            if (request.IsShip.HasValue && request.IsShip.Value && request.DeliveredFee.HasValue)
            {
                existingOrder.DeliveredFee = request.DeliveredFee.Value;
                existingOrder.TotalPrice = existingOrder.OrderPrice + request.DeliveredFee.Value;
            }
            else if (request.IsShip.HasValue && !request.IsShip.Value)
            {
                existingOrder.DeliveredFee = 0;
                existingOrder.TotalPrice = existingOrder.OrderPrice;
            }
            // Cập nhật các thời gian nếu không phải null
            if (request.PendingTime.HasValue)
            {
                existingOrder.PendingTime = request.PendingTime.Value;
            }

            if (request.ApprovedTime.HasValue)
            {
                existingOrder.ApprovedTime = request.ApprovedTime.Value;
            }

            if (request.RevievedTime.HasValue)
            {
                existingOrder.RevievedTime = request.RevievedTime.Value;
            }

            if (request.ProcessingTime.HasValue)
            {
                existingOrder.ProcessingTime = request.ProcessingTime.Value;
            }

            if (request.StoragedTime.HasValue)
            {
                existingOrder.StoragedTime = request.StoragedTime.Value;
            }

            if (request.ShippingTime.HasValue)
            {
                existingOrder.ShippingTime = request.ShippingTime.Value;
            }

            if (request.DeliveredTime.HasValue)
            {
                existingOrder.DeliveredTime = request.DeliveredTime.Value;
            }

            if (request.FinishedTime.HasValue)
            {
                existingOrder.FinishedTime = request.FinishedTime.Value;
            }

            if (request.AbandonedTime.HasValue)
            {
                existingOrder.AbandonedTime = request.AbandonedTime.Value;
            }

            // Cập nhật đơn hàng trong cơ sở dữ liệu
            await _orderRepository.UpdateOrderAsync(existingOrder);
        }


    }
}
