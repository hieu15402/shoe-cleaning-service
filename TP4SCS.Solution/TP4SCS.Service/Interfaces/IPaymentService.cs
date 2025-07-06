using Microsoft.AspNetCore.Http;
using TP4SCS.Library.Models.Request.Payment;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.Payment;

namespace TP4SCS.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<ApiResponse<string?>> CreatePaymentUrlAsync(HttpContext httpContext, int id, PaymentRequest paymentRequest);

        Task<ApiResponse<VnPayResponse>> VnPayExcuteAsync(IQueryCollection collection);

        Task<ApiResponse<MoMoResponse>> MoMoExcuteAsync(IQueryCollection collection);
    }
}
