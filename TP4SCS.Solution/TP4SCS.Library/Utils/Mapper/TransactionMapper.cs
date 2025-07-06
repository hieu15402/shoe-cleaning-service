using Mapster;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Transaction;
using TP4SCS.Library.Utils.StaticClass;

namespace TP4SCS.Library.Utils.Mapper
{
    public class TransactionMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateTransactionRequest, Transaction>()
                .Map(src => src.ProcessTime, otp => DateTime.Now)
                .Map(src => src.Status, otp => StatusConstants.PROCESSING);

            config.NewConfig<UpdateTransactionRequest, Transaction>();
        }
    }
}
