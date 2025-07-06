using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using TP4SCS.Library.Models.Request.Payment;
using TP4SCS.Library.Models.Response.Payment;
using TP4SCS.Library.Utils.Helpers;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class MoMoService : IMoMoService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public MoMoService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> CreatePaymentUrlAsync(MoMoRequest moMoRequest)
        {
            var momo = new MoMoHelpers();

            var orderId = Guid.NewGuid().ToString();

            moMoRequest.Description = "Thanh Toán Cho Mã Đơn: " + moMoRequest.TransactionId;

            var rawData = $"accessKey={_configuration["MoMo:AccessKey"]}" +
                $"&amount={moMoRequest.Balance}" +
                $"&extraData={""}" +
                $"&ipnUrl={_configuration["MoMo:IpnUrl"]}" +
                $"&orderId={orderId}" +
                $"&orderInfo={moMoRequest.Description}" +
                $"&partnerCode={_configuration["MoMo:PartnerCode"]}" +
                $"&redirectUrl={_configuration["MoMo:RedirectUrl"]}" +
                $"&requestId={orderId}" +
                $"&requestType={_configuration["MoMo:RequestType"]}";

            var signature = momo.HmacSHA256(rawData, _configuration["MoMo:SecretKey"]!);

            var requestData = new
            {
                amount = moMoRequest.Balance,
                extraData = "",
                ipnUrl = _configuration["MoMo:IpnUrl"],
                orderId = orderId,
                orderInfo = moMoRequest.Description,
                partnerCode = _configuration["MoMo:PartnerCode"],
                redirectUrl = _configuration["MoMo:RedirectUrl"],
                requestId = orderId,
                requestType = _configuration["MoMo:RequestType"],
                lang = _configuration["MoMo:Language"],
                signature = signature
            };

            var jsonRequest = JsonConvert.SerializeObject(requestData);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://test-payment.momo.vn/v2/gateway/api/create", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);

                if (responseData != null && responseData.ContainsKey("payUrl"))
                {
                    return responseData["payUrl"];
                }
                else
                {
                    throw new Exception("Failed to retrieve payUrl from MoMo API response.");
                }
            }
        }

        public async Task<MoMoResponse> PaymentExecuteAsync(IQueryCollection collection)
        {
            if (collection.TryGetValue("orderInfo", out var orderInfo) && !string.IsNullOrEmpty(orderInfo) &&
                collection.TryGetValue("resultCode", out var resultCode) && !string.IsNullOrEmpty(resultCode))
            {
                // Convert orderInfo to a regular string
                string orderInfoString = orderInfo.ToString();
                string transactionId = orderInfoString.Split(':', 2).Last().Trim();

                if (int.TryParse(transactionId, out int tranId) &&
                    int.TryParse(resultCode, out int resCode))
                {
                    return await Task.FromResult(new MoMoResponse
                    {
                        TransactionId = tranId,
                        MoMoResponseCode = resCode
                    });
                }
            }

            return await Task.FromResult(new MoMoResponse
            {
                TransactionId = 0,
                MoMoResponseCode = int.MaxValue
            });
        }
    }
}
