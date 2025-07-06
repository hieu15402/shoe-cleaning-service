using Mapster;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Account;
using TP4SCS.Library.Models.Request.Address;
using TP4SCS.Library.Models.Response.Address;

namespace TP4SCS.Library.Utils.Mapper
{
    public class AddressMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<AccountAddress, AddressResponse>();

            config.NewConfig<AccountAddress, UpdateAccountRequest>();

            config.NewConfig<CreateAddressRequest, AccountAddress>()
                .Map(dest => dest.Status, src => "AVAILABLE");
        }
    }
}
