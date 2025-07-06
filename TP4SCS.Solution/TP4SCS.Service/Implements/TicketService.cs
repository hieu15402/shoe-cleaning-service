using Mapster;
using MapsterMapper;
using Microsoft.IdentityModel.Tokens;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Ticket;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.Ticket;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Library.Utils.Utils;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ITicketCategoryRepository _ticketCategoryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAssetUrlRepository _assetUrlRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly Util _util;

        public TicketService(ITicketRepository ticketRepository,
            ITicketCategoryRepository ticketCategoryRepository,
            IAccountRepository accountRepository,
            IAssetUrlRepository assetUrlRepository,
            IOrderRepository orderRepository,
            IBusinessRepository businessRepository,
            IEmailService emailService,
            IMapper mapper,
            Util util)
        {
            _ticketRepository = ticketRepository;
            _ticketCategoryRepository = ticketCategoryRepository;
            _accountRepository = accountRepository;
            _assetUrlRepository = assetUrlRepository;
            _orderRepository = orderRepository;
            _businessRepository = businessRepository;
            _emailService = emailService;
            _mapper = mapper;
            _util = util;
        }

        //Cancel Ticket
        public async Task<ApiResponse<TicketResponse>> CancelTicketAsync(int id)
        {
            var oldTicket = await _ticketRepository.GetUpdateTicketByIdAsync(id);

            if (oldTicket == null)
            {
                return new ApiResponse<TicketResponse>("error", 404, "Không Tìm Thấy Đơn Hỗ Trợ!");
            }

            if (oldTicket.Status.Equals(StatusConstants.PROCESSING))
            {
                return new ApiResponse<TicketResponse>("error", 400, "Đơn Hỗ Trợ Đã Vào Xử Lý!");
            }

            oldTicket.Status = StatusConstants.CANCELED;

            try
            {
                await _ticketRepository.UpdateAsync(oldTicket);

                return new ApiResponse<TicketResponse>("success", "Huỷ Đơn Hỗ Trợ Thành Công!", null, 200);
            }
            catch (Exception)
            {
                return new ApiResponse<TicketResponse>("error", 400, "Huỷ Đơn Hỗ Trợ Thất Bại!");
            }
        }

        //Create Child Ticket
        public async Task<ApiResponse<TicketResponse>> CreateChildTicketAsync(int userid, int id, CreateChildTicketRequest createChildTicketRequest)
        {
            var parentTicket = await _ticketRepository.GetUpdateTicketByIdAsync(id);

            if (parentTicket == null)
            {
                return new ApiResponse<TicketResponse>("error", 404, "Không Tìm Thấy Đơn Hỗ Trợ Chính!");
            }

            if (parentTicket.Status.Equals(StatusConstants.CLOSED))
            {
                return new ApiResponse<TicketResponse>("error", 400, "Đơn Hỗ Trợ Đã Đóng!");
            }

            if (parentTicket.Status.Equals(StatusConstants.OPENING))
            {
                return new ApiResponse<TicketResponse>("error", 400, "Đơn Hỗ Trợ Chưa Được Xử Lý!");
            }

            var account = await _accountRepository.GetAccountByIdNoTrackingAsync(userid);

            var newTicket = _mapper.Map<SupportTicket>(createChildTicketRequest);
            newTicket.ParentTicketId = id;
            newTicket.UserId = userid;
            newTicket.CategoryId = parentTicket.CategoryId;

            var newData = new TicketResponse();

            try
            {
                if (createChildTicketRequest.Assets != null)
                {
                    var newAsset = createChildTicketRequest.Assets.Adapt<List<AssetUrl>>();

                    await _ticketRepository.RunInTransactionAsync(async () =>
                    {
                        await _ticketRepository.CreateTicketAsync(newTicket);

                        newData = await _ticketRepository.GetTicketByIdAsync(newTicket.Id);

                        for (int i = 0; i < newAsset.Count; i++)
                        {
                            newAsset[i].TicketId = newTicket.Id;
                        }

                        await _assetUrlRepository.AddAssetUrlsAsync(newAsset);

                        if (parentTicket.UserId == userid)
                        {
                            parentTicket.IsSeen = false;

                            await _ticketRepository.UpdateTicketAsync(parentTicket);
                        }

                        if (!new[] { RoleConstants.OWNER, RoleConstants.CUSTOMER, RoleConstants.EMPLOYEE }
                            .Contains(account!.Role) &&
                            !parentTicket.IsSeen)
                        {
                            parentTicket.IsSeen = true;

                            await _ticketRepository.UpdateTicketAsync(parentTicket);
                        }
                    });
                }
                else
                {
                    await _ticketRepository.RunInTransactionAsync(async () =>
                    {
                        await _ticketRepository.CreateTicketAsync(newTicket);

                        if (parentTicket.UserId == userid)
                        {
                            parentTicket.IsSeen = false;

                            await _ticketRepository.UpdateTicketAsync(parentTicket);
                        }

                        if (!new[] { RoleConstants.OWNER, RoleConstants.CUSTOMER, RoleConstants.EMPLOYEE }
                            .Contains(account!.Role) &&
                            !parentTicket.IsSeen)
                        {
                            parentTicket.IsSeen = true;

                            await _ticketRepository.UpdateTicketAsync(parentTicket);
                        }
                    });

                    newData = await _ticketRepository.GetTicketByIdAsync(newTicket.Id);
                }

                return new ApiResponse<TicketResponse>("success", "Tạo Đơn Hỗ Trợ Thành Công!", newData, 201);
            }
            catch (Exception)
            {
                return new ApiResponse<TicketResponse>("error", 400, "Tạo Đơn Hỗ Trợ Thất Bại!");
            }

        }

        //Create Order Ticket
        public async Task<ApiResponse<TicketResponse>> CreateOrderTicketAsync(int id, CreateOrderTicketRequest createOrderTicketRequest)
        {
            try
            {
                var newTicket = _mapper.Map<SupportTicket>(createOrderTicketRequest);
                newTicket.UserId = id;
                newTicket.CategoryId = await _ticketCategoryRepository.GetOrderTicketCategoryIdAsync();

                var newData = new TicketResponse();

                var newAsset = createOrderTicketRequest.Assets.Adapt<List<AssetUrl>>();

                await _ticketRepository.RunInTransactionAsync(async () =>
                {
                    await _ticketRepository.CreateTicketAsync(newTicket);

                    newData = await _ticketRepository.GetTicketByIdAsync(newTicket.Id);

                    for (int i = 0; i < newAsset.Count; i++)
                    {
                        newAsset[i].TicketId = newTicket.Id;
                    }

                    await _assetUrlRepository.AddAssetUrlsAsync(newAsset);
                });

                return new ApiResponse<TicketResponse>("success", "Tạo Đơn Hỗ Trợ Thành Công!", newData, 201);
            }
            catch (Exception)
            {
                return new ApiResponse<TicketResponse>("error", 400, "Tạo Đơn Hỗ Trợ Thất Bại!");
            }
        }

        //Create Ticket
        public async Task<ApiResponse<TicketResponse>> CreateTicketAsync(int id, CreateTicketRequest createTicketRequest)
        {
            var category = await _ticketCategoryRepository.GetCategoryByIdAsync(createTicketRequest.CategoryId);
            var orderCateId = await _ticketCategoryRepository.GetOrderTicketCategoryIdAsync();

            if (category == null || category.Status.Equals(StatusConstants.UNAVAILABLE) || createTicketRequest.CategoryId == orderCateId)
            {
                return new ApiResponse<TicketResponse>("error", 404, "Không Tìm Thấy Thông Tin Loại Đơn Hỗ Trợ!");
            }

            var newTicket = _mapper.Map<SupportTicket>(createTicketRequest);
            newTicket.UserId = id;

            var newData = new TicketResponse();

            try
            {
                if (createTicketRequest.Assets != null)
                {
                    var newAsset = createTicketRequest.Assets.Adapt<List<AssetUrl>>();

                    await _ticketRepository.RunInTransactionAsync(async () =>
                    {
                        await _ticketRepository.CreateTicketAsync(newTicket);

                        newData = await _ticketRepository.GetTicketByIdAsync(newTicket.Id);

                        for (int i = 0; i < newAsset.Count; i++)
                        {
                            newAsset[i].TicketId = newTicket.Id;
                        }

                        await _assetUrlRepository.AddAssetUrlsAsync(newAsset);
                    });
                }
                else
                {
                    await _ticketRepository.CreateTicketAsync(newTicket);

                    newData = await _ticketRepository.GetTicketByIdAsync(newTicket.Id);
                }

                return new ApiResponse<TicketResponse>("success", "Tạo Đơn Hỗ Trợ Thành Công!", newData, 201);
            }
            catch (Exception)
            {
                return new ApiResponse<TicketResponse>("error", 400, "Tạo Đơn Hỗ Trợ Thất Bại!");
            }
        }

        //Get Ticket By Id
        public async Task<ApiResponse<TicketResponse?>> GetTicketByIdAsync(int id)
        {
            var ticket = await _ticketRepository.GetTicketByIdAsync(id);

            if (ticket == null)
            {
                return new ApiResponse<TicketResponse?>("error", 404, "Không Tìm Thấy Đơn Hỗ Trợ!");
            }

            return new ApiResponse<TicketResponse?>("success", "Lấy Thông Tin Hỗ Trợ Thành Công!", ticket, 200);
        }

        //Get Tickets
        public async Task<ApiResponse<IEnumerable<TicketsResponse>?>> GetTicketsAsync(GetTicketRequest getTicketRequest)
        {
            var (tickets, pagination) = await _ticketRepository.GetTicketsAsync(getTicketRequest);

            if (tickets == null)
            {
                return new ApiResponse<IEnumerable<TicketsResponse>?>("error", 404, "Không Tồn Tại Đơn Hỗ Trợ Nào!");
            }

            return new ApiResponse<IEnumerable<TicketsResponse>?>("success", "Lấy Thông Tin Hỗ Trợ Thành Công", tickets, 200, pagination);
        }

        //Get Tickets By Branch Id
        public async Task<ApiResponse<IEnumerable<TicketsResponse>?>> GetTicketsByBranchIdAsync(int id, GetBusinessTicketRequest getBusinessTicketRequest)
        {
            var (tickets, pagination) = await _ticketRepository.GetTicketsByBranchIdAsync(id, getBusinessTicketRequest);

            if (tickets == null)
            {
                return new ApiResponse<IEnumerable<TicketsResponse>?>("error", 404, "Không Tồn Tại Đơn Hỗ Trợ Nào!");
            }

            return new ApiResponse<IEnumerable<TicketsResponse>?>("success", "Lấy Thông Tin Hỗ Trợ Thành Công", tickets, 200, pagination);
        }

        //Get Tickets By Business Id
        public async Task<ApiResponse<IEnumerable<TicketsResponse>?>> GetTicketsByBusinessAsync(int id, GetBusinessTicketRequest getBusinessTicketRequest)
        {
            var (tickets, pagination) = await _ticketRepository.GetTicketsByBusinessIdAsync(id, getBusinessTicketRequest);

            if (tickets == null)
            {
                return new ApiResponse<IEnumerable<TicketsResponse>?>("error", 404, "Không Tồn Tại Đơn Hỗ Trợ Nào!");
            }

            return new ApiResponse<IEnumerable<TicketsResponse>?>("success", "Lấy Thông Tin Hỗ Trợ Thành Công", tickets, 200, pagination);
        }

        public async Task<ApiResponse<TicketResponse>> NotifyForCustomerAsync(NotifyTicketForCustomerRequest notifyTicketForCustomerRequest)
        {
            var email = await _accountRepository.GetAccountEmailByIdAsync(notifyTicketForCustomerRequest.AccountId);

            if (string.IsNullOrEmpty(email))
            {
                return new ApiResponse<TicketResponse>("error", 404, "Không Tìm Thấy Email!");
            }

            var ticket = await _ticketRepository.GetUpdateTicketByIdAsync(notifyTicketForCustomerRequest.TicketId);

            if (ticket == null)
            {
                return new ApiResponse<TicketResponse>("error", 404, "Không Tìm Thấy Thông Tin Đơn!");
            }

            var childTicket = await _ticketRepository.GetModeratorChildTicketByParentId(ticket.Id);

            string emailSubject = "ShoeCareHub Đơn Khiếu Nại";
            string emailBody = "Đơn Khiếu Nại Của Bạn Đã Được Đưa Ra Quyết Định:\n" +
                $" {childTicket!.Content} \n\n" +
                "Vui Lòng Xem Xét Và Bổ Sung Thông Tin Nếu Chưa Thoả Mãn Trong Vòng 24h Tới!";

            ticket.AutoClosedTime = DateTime.Now.AddDays(1);

            try
            {
                await _ticketRepository.UpdateTicketAsync(ticket);

                await _emailService.SendEmailAsync(email, emailSubject, emailBody);

                return new ApiResponse<TicketResponse>("error", "Gửi Email Thông Báo Cho Người Dùng Thành Công!", null, 200);
            }
            catch (Exception)
            {
                return new ApiResponse<TicketResponse>("error", 400, "Gửi Email Thông Báo Cho Người Dùng Thất Bại!");
            }
        }

        public async Task<ApiResponse<TicketResponse>> NotifyForOwnerAsync(NotifyTicketForOwnerRequest notifyTicketForOwnerRequest)
        {
            var cusEmail = await _accountRepository.GetAccountEmailByIdAsync(notifyTicketForOwnerRequest.AccountId);

            if (string.IsNullOrEmpty(cusEmail))
            {
                return new ApiResponse<TicketResponse>("error", 404, "Không Tìm Thấy Email Khách Hàng!");
            }

            var ticket = await _ticketRepository.GetUpdateTicketByIdAsync(notifyTicketForOwnerRequest.TicketId);

            if (ticket == null)
            {
                return new ApiResponse<TicketResponse>("error", 404, "Không Tìm Thấy Thông Tin Đơn Khiếu Nại!");
            }

            ticket.IsSeen = false;
            ticket.IsOwnerNoti = true;
            ticket.Status = StatusConstants.RESOLVING;

            var order = await _orderRepository.GetUpdateOrderByIdAsync(notifyTicketForOwnerRequest.OrderId);

            if (order == null)
            {
                return new ApiResponse<TicketResponse>("error", 404, "Không Tìm Thấy Thông Tin Đơn Hàng!");
            }

            order.CanceledTime = null;
            order.PendingTime = null;
            order.ApprovedTime = null;
            order.RevievedTime = null;
            order.ProcessingTime = null;
            order.StoragedTime = null;
            order.ShippingTime = null;
            order.DeliveredTime = null;
            order.FinishedTime = null;
            order.AbandonedTime = null;
            order.IsComplained = true;
            order.Status = StatusConstants.APPROVED;

            var owner = await _accountRepository.GetAccountEmailByTicketIdAsync(notifyTicketForOwnerRequest.TicketId);

            if (owner == null)
            {
                return new ApiResponse<TicketResponse>("error", 404, "Không Tìm Thấy Thông Tin Chủ Doanh Nghiệp!");
            }

            var business = await _businessRepository.GetBusinessByOwnerIdNoTrackingAsync(owner.Id);

            if (business == null)
            {
                return new ApiResponse<TicketResponse>("error", 404, "Không Tìm Thấy Thông Tin Doanh Nghiệp!");
            }

            var body = $"Shop {business.Name} Của Bạn Có Đơn Hàng Với Mã Vận Đơn {order.ShippingCode} Cần Xử Lí Khiếu Nại!";

            try
            {
                await _ticketRepository.RunInTransactionAsync(async () =>
                {
                    await _ticketRepository.UpdateTicketAsync(ticket);

                    await _orderRepository.UpdateOrderAsync(order);

                    await _emailService.SendEmailAsync(cusEmail, "ShoeCareHub Đơn Khiếu Nại", "Khiếu Nại Của bạn Đã Được Chấp Thuận Và Đơn Hàng Sẽ Được Xử Lý Lại. Vui Lòng Đóng Đơn Sau Khi Hoàn Tất!");

                    await _emailService.SendEmailAsync(owner.Email, "ShoeCareHub Đơn Khiếu Nại", body);
                });

                return new ApiResponse<TicketResponse>("success", "Gửi Thông Báo Cho Doanh Nghiệp Thành Công!", null);
            }
            catch (Exception)
            {
                return new ApiResponse<TicketResponse>("error", 400, "Gửi Thông Báo Cho Doanh Nghiệp Thất Bại!");
            }
        }

        //Update Ticket Status
        public async Task<ApiResponse<TicketResponse>> UpdateTicketStatusAsync(int moderatorId, int id, UpdateTicketStatusRequest updateTicketStatusRequest)
        {
            var oldTicket = await _ticketRepository.GetUpdateTicketByIdAsync(id);

            if (oldTicket == null)
            {
                return new ApiResponse<TicketResponse>("error", 404, "Không Tìm Thấy Đơn Hỗ Trợ!");
            }

            if (!_util.CheckTicketStatus(updateTicketStatusRequest.Status))
            {
                return new ApiResponse<TicketResponse>("error", 400, "Trạng Thái Không Hợp Lệ!");
            }

            var oldStatus = oldTicket.Status;

            oldTicket.Status = updateTicketStatusRequest.Status.Trim().ToUpperInvariant();

            if (updateTicketStatusRequest.Status.Trim().ToUpperInvariant().Equals(StatusConstants.PROCESSING))
            {
                oldTicket.ModeratorId = moderatorId;
            }

            if (updateTicketStatusRequest.Status.Trim().ToUpperInvariant().Equals(StatusConstants.CLOSED))
            {
                oldTicket.IsSeen = true;
                oldTicket.IsOwnerNoti = false;
            }

            try
            {
                await _ticketRepository.RunInTransactionAsync(async () =>
                {
                    await _ticketRepository.UpdateTicketAsync(oldTicket);

                    string email = await _accountRepository.GetAccountEmailByIdAsync(oldTicket.UserId);
                    string emailSubject = "ShoeCareHub Đơn Khiếu Nại";
                    string emailBody;

                    if (oldStatus.Equals(StatusConstants.OPENING) && updateTicketStatusRequest.Status.Equals(StatusConstants.PROCESSING))
                    {
                        emailBody = "Đơn Khiếu Nại Của Bạn Đã Được Tiếp Nhận Và Đang Được Xử Lý!";
                    }
                    //else if (oldTicket.Status.Equals(StatusConstants.OPENING) && oldTicket.OrderId.HasValue)
                    //{
                    //    emailBody = "Đơn Khiếu Nại Của Bạn Đã Được Tiếp Nhận Và Đang Được Xử Lý!";

                    //    await _emailService.SendEmailAsync(email, emailSubject, emailBody);
                    //}
                    else if (oldStatus.Equals(StatusConstants.OPENING) && oldTicket.Status.Equals(StatusConstants.CLOSED))
                    {
                        emailBody = "Đơn Khiếu Nại Của Bạn Đã Bị Từ Chối Vui Lòng Kiểm Tra Lại Thông Tin Và Thử Lại!";
                    }
                    else
                    {
                        emailBody = "Đơn Khiếu Nại Của Bạn Đã Hoàn Tất Việc Xử Lý!";
                    }

                    await _emailService.SendEmailAsync(email, emailSubject, emailBody);
                });

                return new ApiResponse<TicketResponse>("error", "Cập Nhập Trạng Thái Đơn Hỗ Trợ Thành Công!", null, 200);
            }
            catch (Exception)
            {
                return new ApiResponse<TicketResponse>("error", 400, "Cập Nhập Trạng Thái Đơn Hỗ Trợ Thất Bại!");
            }
        }
    }
}
