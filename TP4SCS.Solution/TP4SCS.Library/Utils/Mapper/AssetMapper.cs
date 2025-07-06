using Mapster;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.AssetUrl;

namespace TP4SCS.Library.Utils.Mapper
{
    internal class AssetMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<AssetUrlRequest, AssetUrl>()
                .Map(dest => dest.BusinessId, src => (int?)null)
                .Map(dest => dest.FeedbackId, src => (int?)null)
                .Map(dest => dest.ServiceId, src => (int?)null)
                .Map(dest => dest.MaterialId, src => (int?)null)
                .Map(dest => dest.TicketId, src => (int?)null);
        }
    }
}
