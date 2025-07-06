using Mapster;
using MapsterMapper;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.PlatformPack;
using TP4SCS.Library.Models.Request.SubscriptionPack;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.SubcriptionPack;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Library.Utils.Utils;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class PlatformPackService : IPlatformPackService
    {
        private readonly IPlatformPackRepository _subscriptionRepository;
        private readonly IMapper _mapper;
        private readonly Util _util;

        public PlatformPackService(IPlatformPackRepository subscriptionRepository, IMapper mapper, Util util)
        {
            _subscriptionRepository = subscriptionRepository;
            _mapper = mapper;
            _util = util;
        }

        public async Task<ApiResponse<PlatformPackResponse>> CreateRegisterPackAsync(RegisterPackRequest subscriptionPackRequest)
        {
            int totalPack = await _subscriptionRepository.CountPackAsync();

            if (totalPack >= 3)
            {
                return new ApiResponse<PlatformPackResponse>("error", 400, "Đã Có Tối Đa 3 Gói Đăng Kí!");
            }

            List<int> periods = await _subscriptionRepository.GetPeriodArrayAsync();

            if (periods.Contains(subscriptionPackRequest.Period))
            {
                return new ApiResponse<PlatformPackResponse>("error", 400, "Thời Hạn Gói Đăng Kí Trùng Lập!");
            }

            var name = _util.FormatStringName(subscriptionPackRequest.Name);

            var isNameExisted = await _subscriptionRepository.IsPackNameExistedAsync(name);

            if (isNameExisted)
            {
                return new ApiResponse<PlatformPackResponse>("error", 400, "Tên Gói Đăng Kí Đã Tồn Tại!");
            }

            periods.Add(subscriptionPackRequest.Period);
            periods.Sort();

            int newIndex = periods.IndexOf(subscriptionPackRequest.Period);

            try
            {
                var newPack = _mapper.Map<PlatformPack>(subscriptionPackRequest);
                newPack.Name = name;
                newPack.Description = "";
                newPack.Feature = "";
                newPack.Type = TypeConstants.REGISTER;

                var secondPack = new PlatformPack();
                var thirdPack = new PlatformPack();

                if (newIndex != 0)
                {
                    decimal basePrice = await _subscriptionRepository.GetPackPriceByPeriodAsync(periods[0]);
                    decimal savePrice = (basePrice / periods[0]) - (subscriptionPackRequest.Price / subscriptionPackRequest.Period);
                    string save = savePrice.ToString("#,0", System.Globalization.CultureInfo.InvariantCulture);

                    if (savePrice > 0)
                    {
                        newPack.Description = $"Tiết Kiệm {save}đ/Tháng";
                    }

                    await _subscriptionRepository.CreatePackAsync(newPack);
                }
                else
                {
                    secondPack = await _subscriptionRepository.GetPackByPeriodAsync(periods[1]) ?? new PlatformPack();
                    thirdPack = await _subscriptionRepository.GetPackByPeriodAsync(periods[2]) ?? new PlatformPack();

                    decimal savePriceOfSecond = (subscriptionPackRequest.Price / subscriptionPackRequest.Period) - (secondPack.Price / secondPack.Period);
                    string saveOfSeccond = savePriceOfSecond.ToString("#,0", System.Globalization.CultureInfo.InvariantCulture);
                    if (savePriceOfSecond > 0)
                    {
                        secondPack.Description = $"Tiết Kiệm {saveOfSeccond}đ/Tháng";
                    }
                    else
                    {
                        secondPack.Description = "";
                    }

                    decimal savePriceOfThird = (subscriptionPackRequest.Price / subscriptionPackRequest.Period) - (thirdPack.Price / thirdPack.Period);
                    string saveOfthird = savePriceOfThird.ToString("#,0", System.Globalization.CultureInfo.InvariantCulture);
                    if (savePriceOfThird > 0)
                    {
                        thirdPack.Description = $"Tiết Kiệm {saveOfthird}đ/Tháng";
                    }
                    else
                    {
                        thirdPack.Description = "";
                    }

                    await _subscriptionRepository.RunInTransactionAsync(async () =>
                    {
                        await _subscriptionRepository.CreatePackAsync(newPack);

                        await _subscriptionRepository.UpdatePackAsync(secondPack);

                        await _subscriptionRepository.UpdatePackAsync(thirdPack);
                    });
                }

                return new ApiResponse<PlatformPackResponse>("success", "Cập Nhập Gói Đăng Kí Thành Công!", null, 200);
            }
            catch (Exception)
            {
                return new ApiResponse<PlatformPackResponse>("error", 400, "Tạo Gói Đăng Kí Mới Thất Bại!");
            }
        }

        public async Task<ApiResponse<PlatformPackResponse>> DeleteRegisterPackAsync(int id)
        {
            var pack = await _subscriptionRepository.GetPackByIdNoTrackingAsync(id);

            if (pack == null)
            {
                return new ApiResponse<PlatformPackResponse>("error", 404, "Không Tìm Thấy Thông Tin Gói Đăng Kí!");
            }

            List<int> periods = await _subscriptionRepository.GetPeriodArrayAsync();
            periods.Remove(pack.Period);
            periods.Sort();

            var basePack = await _subscriptionRepository.GetPackByPeriodAsync(periods[0]) ?? new PlatformPack();
            var remainPack = await _subscriptionRepository.GetPackByPeriodAsync(periods[1]) ?? new PlatformPack();

            decimal savePrice = (basePack.Price / periods[0]) - (remainPack.Price / remainPack.Period);
            string save = savePrice.ToString("#,0", System.Globalization.CultureInfo.InvariantCulture);

            if (savePrice > 0)
            {
                remainPack.Description = $"Tiết Kiệm {save}đ/Tháng";
            }
            else
            {
                remainPack.Description = "";
            }

            basePack.Description = "";

            try
            {
                await _subscriptionRepository.RunInTransactionAsync(async () =>
                {
                    await _subscriptionRepository.UpdatePackAsync(basePack);

                    await _subscriptionRepository.UpdatePackAsync(remainPack);

                    await _subscriptionRepository.DeletePackAsync(id);
                });

                return new ApiResponse<PlatformPackResponse>("success", "Xoá Gói Đăng Kí Thành Công!", null, 200);
            }
            catch (Exception)
            {
                return new ApiResponse<PlatformPackResponse>("error", 400, "Xoá Gói Đăng Kí Thất Bại!");
            }
        }

        public async Task<ApiResponse<IEnumerable<PlatformPackResponse>?>> GetFeaturePacksAsync()
        {
            var packs = await _subscriptionRepository.GetFeaturePacksAsync();

            if (packs == null)
            {
                return new ApiResponse<IEnumerable<PlatformPackResponse>?>("error", 404, "Thông Tin Gói Tính Năng Trống!");
            }

            var data = packs.Adapt<IEnumerable<PlatformPackResponse>>();

            return new ApiResponse<IEnumerable<PlatformPackResponse>?>("success", "Lấy Thông Tin Gói Tính Năng Tành Công!", data, 200);
        }

        public async Task<ApiResponse<PlatformPackResponse?>> GetPackByIdAsync(int id)
        {
            var pack = await _subscriptionRepository.GetPackByIdAsync(id);

            if (pack == null)
            {
                return new ApiResponse<PlatformPackResponse?>("error", 404, "Không Tìm Thấy Thông Tin Gói Đăng Kí!");
            }

            var data = _mapper.Map<PlatformPackResponse>(pack);

            return new ApiResponse<PlatformPackResponse?>("success", "Lấy Thông Tin Gói Đăng Kí Tành Công!", data, 200);
        }

        public async Task<ApiResponse<IEnumerable<PlatformPackResponse>?>> GetRegisterPacksAsync()
        {
            var packs = await _subscriptionRepository.GetRegisterPacksAsync();

            if (packs == null)
            {
                return new ApiResponse<IEnumerable<PlatformPackResponse>?>("error", 404, "Thông Tin Gói Đăng Kí Trống!");
            }

            var data = packs.Adapt<IEnumerable<PlatformPackResponse>>();

            return new ApiResponse<IEnumerable<PlatformPackResponse>?>("success", "Lấy Thông Tin Gói Đăng Kí Tành Công!", data, 200);
        }

        public async Task<ApiResponse<PlatformPackResponse>> UpdateFeaturePackAsync(int id, FeaturePackRequest featurePackRequest)
        {
            var oldPack = await _subscriptionRepository.GetPackByIdAsync(id);

            if (oldPack == null)
            {
                return new ApiResponse<PlatformPackResponse>("error", 404, "Không Tìm Thấy Thông Tin Gói Đăng Kí!");
            }

            oldPack.Price = featurePackRequest.Price;

            try
            {
                await _subscriptionRepository.UpdatePackAsync(oldPack);

                return new ApiResponse<PlatformPackResponse>("success", "Cập Nhập Gói Tính Năng Thành Công!", null, 200);
            }
            catch (Exception)
            {
                return new ApiResponse<PlatformPackResponse>("error", 400, "Cập Nhập Gói Tính Năng Thất Bại!");
            }
        }

        public async Task<ApiResponse<PlatformPackResponse>> UpdateRegisterPackAsync(int id, RegisterPackRequest subscriptionPackRequest)
        {
            var oldPack = await _subscriptionRepository.GetPackByIdAsync(id);

            if (oldPack == null)
            {
                return new ApiResponse<PlatformPackResponse>("error", 404, "Không Tìm Thấy Thông Tin Gói Đăng Kí!");
            }

            List<int> periods = await _subscriptionRepository.GetPeriodArrayAsync();

            if (oldPack.Period != subscriptionPackRequest.Period && periods.Contains(subscriptionPackRequest.Period))
            {
                return new ApiResponse<PlatformPackResponse>("error", 400, "Thời Hạn Gói Đăng Kí Trùng Lập!");
            }

            var name = _util.FormatStringName(subscriptionPackRequest.Name);

            var isNameExisted = await _subscriptionRepository.IsPackNameExistedAsync(name);

            if (oldPack.Name.Equals(name) == false && isNameExisted)
            {
                return new ApiResponse<PlatformPackResponse>("error", 400, "Tên Gói Đăng Kí Đã Tồn Tại!");
            }

            periods.Remove(oldPack.Period);
            periods.Add(subscriptionPackRequest.Period);
            periods.Sort();

            int newIndex = periods.IndexOf(subscriptionPackRequest.Period);

            try
            {
                var newPack = _mapper.Map(subscriptionPackRequest, oldPack);
                var secondPack = new PlatformPack();
                var thirdPack = new PlatformPack();

                if (newIndex != 0)
                {
                    newPack.Name = name;
                    newPack.Description = "";

                    decimal basePrice = await _subscriptionRepository.GetPackPriceByPeriodAsync(periods[0]);
                    decimal savePrice = (basePrice / periods[0]) - (subscriptionPackRequest.Price / subscriptionPackRequest.Period);
                    string save = savePrice.ToString("#,0", System.Globalization.CultureInfo.InvariantCulture);

                    if (savePrice > 0)
                    {
                        newPack.Description = $"Tiết Kiệm {save}đ/Tháng";
                    }

                    await _subscriptionRepository.UpdatePackAsync(newPack);
                }
                else
                {
                    secondPack = await _subscriptionRepository.GetPackByPeriodAsync(periods[1]) ?? new PlatformPack();
                    thirdPack = await _subscriptionRepository.GetPackByPeriodAsync(periods[2]) ?? new PlatformPack();

                    decimal savePriceOfSecond = (subscriptionPackRequest.Price / subscriptionPackRequest.Period) - (secondPack.Price / secondPack.Period);
                    string saveOfSeccond = savePriceOfSecond.ToString("#,0", System.Globalization.CultureInfo.InvariantCulture);
                    if (savePriceOfSecond > 0)
                    {
                        secondPack.Description = $"Tiết Kiệm {saveOfSeccond}đ/Tháng";
                    }
                    else
                    {
                        secondPack.Description = "";
                    }

                    decimal savePriceOfThird = (subscriptionPackRequest.Price / subscriptionPackRequest.Period) - (thirdPack.Price / thirdPack.Period);
                    string saveOfthird = savePriceOfThird.ToString("#,0", System.Globalization.CultureInfo.InvariantCulture);
                    if (savePriceOfThird > 0)
                    {
                        thirdPack.Description = $"Tiết Kiệm {saveOfthird}đ/Tháng";
                    }
                    else
                    {
                        thirdPack.Description = "";
                    }

                    await _subscriptionRepository.RunInTransactionAsync(async () =>
                    {
                        await _subscriptionRepository.UpdatePackAsync(secondPack);

                        await _subscriptionRepository.UpdatePackAsync(thirdPack);

                        await _subscriptionRepository.UpdatePackAsync(newPack);
                    });
                }

                return new ApiResponse<PlatformPackResponse>("success", "Cập Nhập Gói Đăng Kí Thành Công!", null, 200);
            }
            catch (Exception)
            {
                return new ApiResponse<PlatformPackResponse>("error", 400, "Cập Nhập Gói Đăng Kí Thất Bại!");
            }
        }
    }
}
