using Mapster;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Category;
using TP4SCS.Library.Models.Response.Category;
using TP4SCS.Library.Utils.StaticClass;

namespace TP4SCS.Library.Utils.Mapper
{
    public class TicketCategoryMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<TicketCategory, TicketCategoryResponse>();

            config.NewConfig<TicketCategoryRequest, TicketCategory>()
                .Map(dest => dest.Status, otp => StatusConstants.AVAILABLE);
        }
    }
}
