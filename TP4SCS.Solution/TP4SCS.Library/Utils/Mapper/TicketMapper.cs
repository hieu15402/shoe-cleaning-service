using Mapster;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Ticket;
using TP4SCS.Library.Utils.StaticClass;

namespace TP4SCS.Library.Utils.Mapper
{
    public class TicketMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateTicketRequest, SupportTicket>()
                .Map(dest => dest.ModeratorId, otp => (int?)null)
                .Map(dest => dest.OrderId, otp => (int?)null)
                .Map(dest => dest.ParentTicketId, otp => (int?)null)
                .Map(dest => dest.CreateTime, otp => DateTime.Now)
                .Map(dest => dest.IsParentTicket, otp => true)
                .Map(dest => dest.IsSeen, otp => true)
                .Map(dest => dest.IsOwnerNoti, otp => false)
                .Map(dest => dest.AutoClosedTime, otp => DateTime.Now)
                .Map(dest => dest.Status, otp => StatusConstants.OPENING);

            config.NewConfig<CreateOrderTicketRequest, SupportTicket>()
                .Map(dest => dest.ModeratorId, otp => (int?)null)
                .Map(dest => dest.ParentTicketId, otp => (int?)null)
                .Map(dest => dest.CreateTime, otp => DateTime.Now)
                .Map(dest => dest.IsParentTicket, otp => true)
                .Map(dest => dest.IsSeen, otp => true)
                .Map(dest => dest.IsOwnerNoti, otp => false)
                .Map(dest => dest.AutoClosedTime, otp => DateTime.Now)
                .Map(dest => dest.Status, otp => StatusConstants.OPENING);

            config.NewConfig<CreateChildTicketRequest, SupportTicket>()
                .Map(dest => dest.ModeratorId, otp => (int?)null)
                .Map(dest => dest.OrderId, otp => (int?)null)
                .Map(dest => dest.CreateTime, otp => DateTime.Now)
                .Map(dest => dest.IsParentTicket, otp => false)
                .Map(dest => dest.IsSeen, otp => true)
                .Map(dest => dest.IsOwnerNoti, otp => false)
                .Map(dest => dest.AutoClosedTime, otp => DateTime.Now)
                .Map(dest => dest.Status, otp => StatusConstants.CLOSED);
        }
    }
}
