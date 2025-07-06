using Microsoft.AspNetCore.Http;
using TP4SCS.Library.Models.Request.Payment;
using TP4SCS.Library.Models.Response.Payment;

namespace TP4SCS.Services.Interfaces
{
    public interface IMoMoService
    {
        Task<string> CreatePaymentUrlAsync(MoMoRequest moMoRequest);

        Task<MoMoResponse> PaymentExecuteAsync(IQueryCollection collection);
    }
}
