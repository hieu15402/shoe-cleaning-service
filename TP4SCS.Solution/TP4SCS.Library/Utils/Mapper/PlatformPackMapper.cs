using Mapster;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.SubscriptionPack;
using TP4SCS.Library.Models.Response.SubcriptionPack;

namespace TP4SCS.Library.Utils.Mapper
{
    public class PlatformPackMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<PlatformPack, PlatformPackResponse>();

            config.NewConfig<RegisterPackRequest, PlatformPack>();
        }
    }
}
