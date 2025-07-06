using Microsoft.EntityFrameworkCore;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Notification;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Repository.Interfaces;

namespace TP4SCS.Repository.Implements
{
    public class OrderNotificationRepository : GenericRepository<OrderNotification>, IOrderNotificationRepository
    {
        public OrderNotificationRepository(Tp4scsDevDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task CreateOrderNotificationAsync(OrderNotification orderNotification)
        {
            await InsertAsync(orderNotification);
        }

        public async Task DeleteOrderNotificationAsync(int id)
        {
            await DeleteAsync(id);
        }

        public async Task<OrderNotification?> GetOrderNotificationByIdAsync(int id)
        {
            return await _dbContext.OrderNotifications.AsNoTracking().SingleOrDefaultAsync(n => n.Id == id);
        }

        public async Task<(IEnumerable<OrderNotification>?, Pagination)> GetOrderNotificationsAsync(GetOrderNotificationRequest getOrderNotificationRequest)
        {
            var notifiesQuery = _dbContext.OrderDetails
                .AsNoTracking()
                .Include(n => n.Order)
                .ThenInclude(n => n.Account.Id)
                .Include(n => n.Branch)
                .ThenInclude(n => n.Business.Id)
                .Include(n => n.Order)
                .ThenInclude(n => n.OrderNotifications)
                .AsQueryable();

            switch (getOrderNotificationRequest.IdType)
            {
                case IdOption.ACCOUNT:
                    notifiesQuery = notifiesQuery
                        .Where(n => n.Order.AccountId == getOrderNotificationRequest.Id &&
                        n.Order.OrderNotifications.Any(on => on.IsProviderNoti) == false)
                        .AsQueryable();
                    break;
                case IdOption.BUSINESS:
                    notifiesQuery = notifiesQuery
                        .Where(n => n.Branch.BusinessId == getOrderNotificationRequest.Id &&
                        n.Order.OrderNotifications.Any(on => on.IsProviderNoti) == true)
                        .AsQueryable();
                    break;
                case IdOption.BRANCH:
                    notifiesQuery = notifiesQuery
                        .Where(n => n.Branch.Id == getOrderNotificationRequest.Id &&
                        n.Order.OrderNotifications.Any(on => on.IsProviderNoti) == true)
                        .AsQueryable();
                    break;
                default:
                    break;
            }

            var notifies = notifiesQuery
                .OrderBy(n => n.Order.OrderNotifications.OrderByDescending(on => on.NotificationTime))
                .SelectMany(n => n.Order.OrderNotifications)
                .AsQueryable();

            //Count Total Data
            int totalData = await notifies.AsNoTracking().CountAsync();

            //Paging
            int skipNum = (getOrderNotificationRequest.PageNum - 1) * getOrderNotificationRequest.PageSize;
            notifies = notifies.Skip(skipNum).Take(getOrderNotificationRequest.PageSize);

            var result = await notifies.ToListAsync();

            int totalPage = (int)Math.Ceiling((decimal)totalData / getOrderNotificationRequest.PageSize);
            var pagination = new Pagination(totalData, getOrderNotificationRequest.PageSize, getOrderNotificationRequest.PageNum, totalPage);

            return (result, pagination);
        }
    }
}
