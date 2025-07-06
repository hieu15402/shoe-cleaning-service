using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TP4SCS.Library.Models.Request.ShipFee;
using TP4SCS.Library.Models.Response.Location;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class ShipService : IShipService
    {
        private readonly IConfiguration _configuration;
        private readonly IOrderRepository _orderRepository;

        public ShipService(IConfiguration configuration, IOrderRepository orderRepository)
        {
            _configuration = configuration;
            _orderRepository = orderRepository;
        }

        //Get Available Services
        public async Task<List<AvailableService>?> GetAvailableServicesAsync(HttpClient httpClient, int fromDistrict, int toDistrict)
        {
            try
            {
                if (!httpClient.DefaultRequestHeaders.Contains("Token"))
                {
                    httpClient.DefaultRequestHeaders.Add("Token", _configuration["GHN_API:ApiToken"]);
                }

                var requestBody = new
                {
                    shop_id = 195216,
                    from_district = fromDistrict,
                    to_district = toDistrict
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(_configuration["GHN_API:AvailableServicesUrl"], content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(responseBody);
                var services = document.RootElement
                    .GetProperty("data")
                    .EnumerateArray()
                    .Select(service => new AvailableService
                    {
                        ServiceID = service.GetProperty("service_id").GetInt32(),
                        ShortName = service.GetProperty("short_name").GetString() ?? string.Empty,
                        ServiceTypeID = service.GetProperty("service_type_id").GetInt32()
                    })
                    .ToList();

                return services;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task<string?> GetDistrictNamByIdAsync(HttpClient httpClient, int id)
        {
            try
            {
                if (!httpClient.DefaultRequestHeaders.Contains("Token"))
                {
                    httpClient.DefaultRequestHeaders.Add("Token", _configuration["GHN_API:ApiToken"]);
                }

                // Gửi request đến API
                var url = _configuration["GHN_API:DistrictUrl"];
                var response = await httpClient.GetStringAsync(url);

                using var document = JsonDocument.Parse(response);
                var districtName = document.RootElement
                    .GetProperty("data")
                    .EnumerateArray()
                    .Where(district => district.GetProperty("DistrictID").GetInt32() == id)
                    .Select(district => district.GetProperty("DistrictName").GetString() ?? string.Empty)
                    .SingleOrDefault();

                return districtName;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Get District By Province Id
        public async Task<List<District>?> GetDistrictsByProvinceIdAsync(HttpClient httpClient, int provinceId)
        {
            try
            {
                if (!httpClient.DefaultRequestHeaders.Contains("Token"))
                {
                    httpClient.DefaultRequestHeaders.Add("Token", _configuration["GHN_API:ApiToken"]);
                }

                // Gửi request đến API
                var url = _configuration["GHN_API:DistrictUrl"] + "?province_id=" + provinceId;
                var response = await httpClient.GetStringAsync(url);

                using var document = JsonDocument.Parse(response);
                var provinces = document.RootElement
                    .GetProperty("data")
                    .EnumerateArray()
                    .Select(district => new District
                    {
                        ProvinceID = district.GetProperty("ProvinceID").GetInt32(),
                        DistrictID = district.GetProperty("DistrictID").GetInt32(),
                        DistrictName = district.GetProperty("DistrictName").GetString() ?? string.Empty,
                        NameExtension = district.GetProperty("NameExtension").EnumerateArray().Select(x => x.GetString() ?? string.Empty).ToList()
                    })
                    .ToList();

                return provinces;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string?> GetProvinceNameByIdAsync(HttpClient httpClient, int id)
        {
            try
            {
                if (!httpClient.DefaultRequestHeaders.Contains("Token"))
                {
                    httpClient.DefaultRequestHeaders.Add("Token", _configuration["GHN_API:ApiToken"]);
                }

                // Gửi request đến API
                var response = await httpClient.GetStringAsync(_configuration["GHN_API:ProvinceUrl"]);

                // Parse dữ liệu JSON và lấy danh sách Province
                using var document = JsonDocument.Parse(response);
                var provinceName = document.RootElement
                    .GetProperty("data")
                    .EnumerateArray()
                    .Where(province => province.GetProperty("ProvinceID").GetInt32() == id)
                    .Select(province => province.GetProperty("ProvinceName").GetString() ?? string.Empty)
                    .SingleOrDefault();

                return provinceName;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Get Provinces
        public async Task<List<Province>?> GetProvincesAsync(HttpClient httpClient)
        {
            try
            {
                if (!httpClient.DefaultRequestHeaders.Contains("Token"))
                {
                    httpClient.DefaultRequestHeaders.Add("Token", _configuration["GHN_API:ApiToken"]);
                }

                // Gửi request đến API
                var response = await httpClient.GetStringAsync(_configuration["GHN_API:ProvinceUrl"]);

                // Parse dữ liệu JSON và lấy danh sách Province
                using var document = JsonDocument.Parse(response);
                var provinces = document.RootElement
                    .GetProperty("data")
                    .EnumerateArray()
                    .Select(province => new Province
                    {
                        ProvinceID = province.GetProperty("ProvinceID").GetInt32(),
                        ProvinceName = province.GetProperty("ProvinceName").GetString() ?? string.Empty,
                        NameExtension = province.GetProperty("NameExtension").EnumerateArray().Select(x => x.GetString() ?? string.Empty).ToList()
                    })
                    .ToList();

                return provinces;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Get Shipping Fee
        public async Task<decimal> GetShippingFeeAsync(HttpClient httpClient, GetShipFeeRequest getShipFeeRequest)
        {
            try
            {
                if (!httpClient.DefaultRequestHeaders.Contains("Token"))
                {
                    httpClient.DefaultRequestHeaders.Add("Token", _configuration["GHN_API:ApiToken"]);
                }

                int heightPerBox = 15;
                int lengthPerBox = 35;
                int widthPerBox = 25;
                int weightPerBox = 400;

                var availableServices = await GetAvailableServicesAsync(httpClient, getShipFeeRequest.FromDistricId, getShipFeeRequest.ToDistricId);

                int? serviceTypeId = availableServices?.SingleOrDefault(s => s.ServiceTypeID == 2)?.ServiceTypeID
                                   ?? availableServices?.SingleOrDefault(s => s.ServiceTypeID == 5)?.ServiceTypeID;

                if (!serviceTypeId.HasValue)
                {
                    throw new InvalidOperationException($"Không hỗ trợ ship từ DistricId: {getShipFeeRequest.FromDistricId} tới FromDistricId: {getShipFeeRequest.ToDistricId}");
                }

                var requestBody = new
                {
                    service_type_id = serviceTypeId.Value,
                    from_district_id = getShipFeeRequest.FromDistricId,
                    from_ward_code = getShipFeeRequest.FromWardCode,
                    to_district_id = getShipFeeRequest.ToDistricId,
                    to_ward_code = getShipFeeRequest.ToWardCode,
                    height = heightPerBox,
                    length = lengthPerBox,
                    weight = weightPerBox,
                    width = widthPerBox,
                    insurance_value = 0,
                    coupon = (string?)null
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(_configuration["GHN_API:ShipFeeUrl"], content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(responseBody);

                var totalFee = document.RootElement
                    .GetProperty("data")
                    .GetProperty("total");

                return totalFee.GetDecimal();
            }
            catch (Exception)
            {
                throw new InvalidOperationException($"Không hỗ trợ ship từ DistricId: {getShipFeeRequest.FromDistricId} tới FromDistricId: {getShipFeeRequest.ToDistricId}");
            }
        }

        public async Task<string?> GetWardNameByWardCodeAsync(HttpClient httpClient, int districtId, string code)
        {
            try
            {
                if (!httpClient.DefaultRequestHeaders.Contains("Token"))
                {
                    httpClient.DefaultRequestHeaders.Add("Token", _configuration["GHN_API:ApiToken"]);
                }

                var requestData = new { district_id = districtId };
                var response = await httpClient.PostAsJsonAsync(_configuration["GHN_API:WardUrl"], requestData);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Failed to fetch wards: {response.ReasonPhrase}");
                }

                var responseData = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(responseData);
                var wardName = document.RootElement
                    .GetProperty("data")
                    .EnumerateArray()
                    .Where(ward => (ward.GetProperty("WardCode").GetString() ?? string.Empty).Equals(code, StringComparison.OrdinalIgnoreCase))
                    .Select(ward => ward.GetProperty("WardName").GetString() ?? string.Empty)
                    .SingleOrDefault();

                return wardName;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Get Ward By District Id
        public async Task<List<Ward>?> GetWardsByDistrictIdAsync(HttpClient httpClient, int districtId)
        {
            try
            {
                if (!httpClient.DefaultRequestHeaders.Contains("Token"))
                {
                    httpClient.DefaultRequestHeaders.Add("Token", _configuration["GHN_API:ApiToken"]);
                }

                var requestData = new { district_id = districtId };
                var response = await httpClient.PostAsJsonAsync(_configuration["GHN_API:WardUrl"], requestData);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Failed to fetch wards: {response.ReasonPhrase}");
                }

                var responseData = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(responseData);
                var wards = document.RootElement
                    .GetProperty("data")
                    .EnumerateArray()
                    .Select(ward => new Ward
                    {
                        WardCode = ward.GetProperty("WardCode").GetString() ?? string.Empty,
                        DistrictID = ward.GetProperty("DistrictID").GetInt32(),
                        WardName = ward.GetProperty("WardName").GetString() ?? string.Empty,
                        NameExtension = ward.GetProperty("NameExtension").EnumerateArray().Select(x => x.GetString() ?? string.Empty).ToList()
                    })
                    .ToList();

                return wards;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string?> CreateShippingOrderAsync(HttpClient httpClient, ShippingOrderRequest request)
        {
            try
            {
                if (!httpClient.DefaultRequestHeaders.Contains("Token"))
                {
                    httpClient.DefaultRequestHeaders.Add("Token", _configuration["GHN_API:ApiToken"]);
                }

                if (!httpClient.DefaultRequestHeaders.Contains("ShopId"))
                {
                    httpClient.DefaultRequestHeaders.Add("ShopId", _configuration["GHN_API:ShopId"]);
                }

                // Tạo nội dung body
                var body = new
                {
                    payment_type_id = 2,
                    required_note = "KHONGCHOXEMHANG",
                    from_name = request.FromName,
                    from_phone = request.FromPhone,
                    from_address = request.FromAddress,
                    from_ward_name = request.FromWardName,
                    from_district_name = request.FromDistrictName,
                    from_province_name = request.FromProvinceName,
                    to_name = request.ToName,
                    to_phone = request.ToPhone,
                    to_address = request.ToAddress,
                    to_ward_code = request.ToWardCode,
                    to_district_id = request.ToDistrictId,
                    cod_amount = request.CODAmount,
                    content = "Giao hàng cho ShoeCareHub",
                    weight = 30,
                    length = 30,
                    width = 40,
                    height = 20,
                    service_id = 0,
                    service_type_id = 2,
                    pick_shift = new[] { 2 }
                };

                // Convert body thành JSON
                var jsonBody = JsonSerializer.Serialize(body);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // Gửi request POST
                var response = await httpClient.PostAsync(_configuration["GHN_API:CreateOrder"], content);

                // Kiểm tra response
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($"Request failed with status code {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
                }

                // Lấy response content
                var responseContent = await response.Content.ReadAsStringAsync();

                // Parse JSON để lấy order_code
                using var document = JsonDocument.Parse(responseContent);
                var orderCode = document.RootElement
                    .GetProperty("data")
                    .GetProperty("order_code")
                    .GetString();

                return orderCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tạo đơn hàng: {ex.Message}");
                return null;
            }
        }

        public async Task<(string? Status, List<(string Status, string UpdatedDate)> Logs)> GetOrderStatusAsync(HttpClient httpClient, string orderCode)
        {
            try
            {
                // Thiết lập headers nếu chưa có
                if (!httpClient.DefaultRequestHeaders.Contains("Token"))
                {
                    httpClient.DefaultRequestHeaders.Add("Token", _configuration["GHN_API:ApiToken"]);
                }

                // Body của request
                var body = new { order_code = orderCode };
                var jsonBody = JsonSerializer.Serialize(body);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                // Gửi request POST
                var response = await httpClient.PostAsync(_configuration["GHN_API:GetOrderDetail"], content);

                // Kiểm tra trạng thái
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($"Request failed with status code {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
                }

                // Parse response
                var responseContent = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(responseContent);

                // Lấy đối tượng data từ JSON response
                var dataElement = document.RootElement.GetProperty("data");

                // Lấy giá trị `status`
                var status = dataElement.GetProperty("status").GetString();
                List<(string Status, string UpdatedDate)> logs = new List<(string, string)>(); // Khởi tạo danh sách logs rỗng mặc định

                if (status != null && status.Equals("ready_to_pick"))
                {
                    // Nếu trạng thái là "ready_to_pick", trả về logs rỗng
                    logs = new List<(string, string)>();
                }
                else
                {
                    // Lấy giá trị `log` và chuyển đổi thành danh sách
                    logs = dataElement.GetProperty("log")
                        .EnumerateArray()
                        .Select(log => (
                            Status: log.GetProperty("status").GetString() ?? "", // Thay thế giá trị null bằng "Unknown"
                            UpdatedDate: log.GetProperty("updated_date").GetString() ?? "" // Thay thế giá trị null bằng "Unknown"
                        ))
                        .ToList();
                }

                // Kiểm tra xem có bất kỳ log nào có status là "delivered" và logs không rỗng
                if (logs.Any(log => log.Status == "delivered") && logs.Count > 0)
                {
                    var order = await _orderRepository.GetOrderByCodeAsync(orderCode);
                    if (order != null && order.Status == StatusConstants.SHIPPING)
                    {
                        order.Status = StatusConstants.DELIVERED;
                        await _orderRepository.UpdateOrderAsync(order);
                    }
                }

                return (status, logs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy trạng thái đơn hàng: {ex.Message}");
                return (null, new List<(string, string)>());
            }
        }
    }
}
