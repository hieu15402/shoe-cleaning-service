using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TP4SCS.Library.Models.Request.Payment;
using TP4SCS.Library.Models.Response.Payment;
using TP4SCS.Library.Utils.Healpers;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;

        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> CreatePaymentUrlAsync(HttpContext httpContext, VnPayRequest vnPayRequest)
        {
            return await Task.Run(() =>
            {
                var vnpay = new VnPayHelpers();
                vnpay.AddRequestData("vnp_Version", _configuration["VnPay:Version"]!);
                vnpay.AddRequestData("vnp_Command", _configuration["VnPay:Command"]!);
                vnpay.AddRequestData("vnp_TmnCode", _configuration["VnPay:TmnCode"]!);
                vnpay.AddRequestData("vnp_Amount", (vnPayRequest.Balance * 100).ToString());
                vnpay.AddRequestData("vnp_CreateDate", vnPayRequest.CreatedDate.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", _configuration["VnPay:CurrCode"]!);
                vnpay.AddRequestData("vnp_IpAddr", VnPayUtils.GetIpAddress(httpContext));
                vnpay.AddRequestData("vnp_Locale", _configuration["VnPay:Locale"]!);
                vnpay.AddRequestData("vnp_OrderInfo", "Thanh Toán Cho Mã Đơn: " + vnPayRequest.TransactionId);
                vnpay.AddRequestData("vnp_OrderType", "other");
                vnpay.AddRequestData("vnp_ReturnUrl", _configuration["VnPay:ReturnUrl"]!);
                vnpay.AddRequestData("vnp_TxnRef", Guid.NewGuid().ToString());

                return vnpay.CreateRequestUrl(_configuration["VnPay:BaseUrl"]!, _configuration["VnPay:HashSecret"]!);
            });
        }

        public async Task<VnPayResponse> PaymentExecuteAsync(IQueryCollection collection)
        {
            return await Task.Run(() =>
            {
                var vnpay = new VnPayHelpers();
                foreach (var (key, value) in collection)
                {
                    if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(key, value.ToString());
                    }
                }

                string vnpOrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
                string transactionId = vnpOrderInfo.Split(':', 2).Last().Trim();

                int tranId = Convert.ToInt32(transactionId);
                var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                var vnp_SecureHash = collection.FirstOrDefault(p => p.Key.Equals("vnp_SecureHash")).Value;

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash!, _configuration["VnPay:HashSecret"]!);

                if (!checkSignature)
                {
                    return new VnPayResponse
                    {
                        TransactionId = tranId,
                        VnPayResponseCode = vnp_ResponseCode
                    };
                }

                return new VnPayResponse
                {
                    TransactionId = tranId,
                    VnPayResponseCode = vnp_ResponseCode
                };
            });
        }
    }
}
