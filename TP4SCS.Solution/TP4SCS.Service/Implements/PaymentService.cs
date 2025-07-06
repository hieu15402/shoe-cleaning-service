using MapsterMapper;
using Microsoft.AspNetCore.Http;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Payment;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.Payment;
using TP4SCS.Library.Repositories;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class PaymentService : IPaymentService
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IPlatformPackRepository _platformPackRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IPackSubscriptionRepository _packSubscriptionRepository;
        private readonly IVnPayService _vnPayService;
        private readonly IMoMoService _moMoService;
        private readonly IMapper _mapper;

        public PaymentService(IBusinessRepository businessRepository,
            IPlatformPackRepository subscriptionPackRepository,
            ITransactionRepository transactionRepository,
            IPackSubscriptionRepository packSubscriptionRepository,
            IVnPayService vnPayService,
            IMoMoService moMoService,
            IMapper mapper)
        {
            _businessRepository = businessRepository;
            _platformPackRepository = subscriptionPackRepository;
            _transactionRepository = transactionRepository;
            _packSubscriptionRepository = packSubscriptionRepository;
            _vnPayService = vnPayService;
            _moMoService = moMoService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<string?>> CreatePaymentUrlAsync(HttpContext httpContext, int id, PaymentRequest paymentRequest)
        {
            if (!paymentRequest.Payment.ToString().Equals(PaymentOptions.VnPay.ToString()) && !paymentRequest.Payment.ToString().Equals(PaymentOptions.MoMo.ToString()))
            {
                return new ApiResponse<string?>("error", 400, "Phương Thức Thanh Toán Không Hợp Lệ!");
            }

            var business = await _businessRepository.GetBusinessByOwnerIdAsync(id);

            if (business == null)
            {
                return new ApiResponse<string?>("error", 404, "Không Tìm Thấy Thông Tin Doanh Nghiệp!");
            }

            var pack = await _platformPackRepository.GetPackByIdNoTrackingAsync(paymentRequest.PackId);

            if (pack == null)
            {
                return new ApiResponse<string?>("error", 404, "Không Tìm Thấy Thông Tin Gói Đăng Kí!");
            }

            if (pack.Type.Equals(TypeConstants.FEATURE))
            {
                var featureErrorMessages = new Dictionary<string, Func<bool>>
                {
                    { FeatureConstants.BUSINESS, () => !business.IsIndividual },
                    { FeatureConstants.MATERIAL, () => business.IsMaterialSupported },
                    { FeatureConstants.SERVICE, () => !business.IsLimitServiceNum }
                };

                if (featureErrorMessages.TryGetValue(pack.Feature!, out var condition) && condition())
                {
                    return new ApiResponse<string?>("error", 400, "Doanh Nghiệp Đã Đăng Kí Gói Tính Năng Này!");
                }
            }

            var newTransaction = new Transaction
            {
                AccountId = id,
                PackName = pack.Name,
                Balance = pack.Price,
                ProcessTime = DateTime.Now,
                Description = $"Thanh Toán {pack.Name} Bằng {paymentRequest.Payment}",
                PaymentMethod = paymentRequest.Payment switch
                {
                    PaymentOptions.VnPay => "VnPay",
                    _ => "MoMo"
                },
                Status = StatusConstants.PROCESSING
            };

            string payUrl = "";

            try
            {
                await _platformPackRepository.RunInTransactionAsync(async () =>
                {
                    await _transactionRepository.CreateTransactionAsync(newTransaction);

                    if (paymentRequest.Payment.Equals(PaymentOptions.VnPay))
                    {

                        var vnpay = new VnPayRequest
                        {
                            TransactionId = newTransaction.Id,
                            Balance = (double)newTransaction.Balance,
                            CreatedDate = DateTime.Now,
                            Description = newTransaction.Description,
                        };

                        payUrl = await _vnPayService.CreatePaymentUrlAsync(httpContext, vnpay);
                    }
                    else
                    {
                        var momo = new MoMoRequest
                        {
                            TransactionId = newTransaction.Id,
                            Balance = (long)newTransaction.Balance,
                            Description = "",
                        };

                        payUrl = await _moMoService.CreatePaymentUrlAsync(momo);
                    }

                    newTransaction.Status = StatusConstants.PROCESSING;

                    await _transactionRepository.UpdateTransactionAsync(newTransaction);
                });

                return new ApiResponse<string?>("success", "Tạo Đường Dẫn Thanh Toán Gói Đăng Kí Thành Công!", payUrl, 201);
            }
            catch (Exception)
            {
                return new ApiResponse<string?>("error", 400, "Tạo Đường Dẫn Thanh Toán Gói Đăng Kí Thất Bại!");
            }
        }

        public async Task<ApiResponse<MoMoResponse>> MoMoExcuteAsync(IQueryCollection collection)
        {
            var result = await _moMoService.PaymentExecuteAsync(collection);

            var transaction = await _transactionRepository.GetTransactionByIdAsync(result.TransactionId);

            if (transaction == null)
            {
                return new ApiResponse<MoMoResponse>("error", 404, "Không Tìm Thấy Thông Tin Giao Dịch!");
            }

            var newTransaction = _mapper.Map<Transaction>(transaction);

            var business = await _businessRepository.GetBusinessByOwnerIdAsync(transaction.AccountId);

            if (business == null)
            {
                return new ApiResponse<MoMoResponse>("error", 404, "Không Tìm Thấy Thông Tin Doanh Nghiệp!");
            }

            var pack = await _platformPackRepository.GetPackByNameAsync(transaction.PackName);

            if (pack == null)
            {
                return new ApiResponse<MoMoResponse>("error", 404, "Không Tìm Thấy Thông Tin Gói Đăng Kí!");
            }

            if (result.MoMoResponseCode == 0)
            {
                await _transactionRepository.RunInTransactionAsync(async () =>
                {
                    newTransaction.Status = StatusConstants.COMPLETED;

                    await _transactionRepository.UpdateTransactionAsync(newTransaction);

                    business.RegisteredTime = DateTime.Now;
                    if (business.ExpiredTime > DateTime.Now)
                    {
                        business.ExpiredTime = business.ExpiredTime.AddMonths(pack.Period);
                    }
                    else
                    {
                        business.ExpiredTime = DateTime.Now.AddMonths(pack.Period);
                    }

                    business.Status = StatusConstants.ACTIVE;

                    if (pack.Type.Equals(TypeConstants.FEATURE))
                    {
                        PackSubscription newSubscription = new PackSubscription
                        {
                            BusinessId = business.Id,
                            PackId = pack.Id,
                            SubscriptionTime = DateTime.Now,
                        };

                        switch (pack.Feature)
                        {
                            case FeatureConstants.BUSINESS:
                                business.IsIndividual = false;
                                break;
                            case FeatureConstants.MATERIAL:
                                business.IsMaterialSupported = true;
                                break;
                            case FeatureConstants.SERVICE:
                                business.IsLimitServiceNum = false;
                                break;
                        }

                        await _packSubscriptionRepository.CreatePackSubscriptionAsync(newSubscription);
                    }

                    await _businessRepository.UpdateBusinessProfileAsync(business);

                    await _businessRepository.SaveAsync();
                });

                return new ApiResponse<MoMoResponse>("success", "Thanh Toán Gói Đăng Kí Thành Công!", null, 200);
            }
            else
            {
                transaction.Status = StatusConstants.FAILED;

                await _transactionRepository.UpdateTransactionAsync(newTransaction);

                await _transactionRepository.SaveAsync();

                return new ApiResponse<MoMoResponse>("error", 400, "Thanh Toán Gói Đăng Kí Thất Bại!");
            }
        }

        public async Task<ApiResponse<VnPayResponse>> VnPayExcuteAsync(IQueryCollection collection)
        {
            var result = await _vnPayService.PaymentExecuteAsync(collection);

            var transaction = await _transactionRepository.GetTransactionByIdAsync(result.TransactionId);

            if (transaction == null)
            {
                return new ApiResponse<VnPayResponse>("error", 404, "Không Tìm Thấy Thông Tin Giao Dịch!");
            }

            var newTransaction = _mapper.Map<Transaction>(transaction);

            var business = await _businessRepository.GetBusinessByOwnerIdAsync(transaction.AccountId);

            if (business == null)
            {
                return new ApiResponse<VnPayResponse>("error", 404, "Không Tìm Thấy Thông Tin Doanh Nghiệp!");
            }

            var pack = await _platformPackRepository.GetPackByNameAsync(transaction.PackName);

            if (pack == null)
            {
                return new ApiResponse<VnPayResponse>("error", 404, "Không Tìm Thấy Thông Tin Gói Đăng Kí!");
            }

            if (result.VnPayResponseCode.Equals("00"))
            {
                await _transactionRepository.RunInTransactionAsync(async () =>
                {
                    newTransaction.Status = StatusConstants.COMPLETED;

                    await _transactionRepository.UpdateTransactionAsync(newTransaction);

                    business.RegisteredTime = DateTime.Now;
                    if (business.ExpiredTime > DateTime.Now)
                    {
                        business.ExpiredTime = business.ExpiredTime.AddMonths(pack.Period);
                    }
                    else
                    {
                        business.ExpiredTime = DateTime.Now.AddMonths(pack.Period);
                    }

                    business.Status = StatusConstants.ACTIVE;

                    if (pack.Type.Equals(TypeConstants.FEATURE))
                    {
                        PackSubscription newSubscription = new PackSubscription
                        {
                            BusinessId = business.Id,
                            PackId = pack.Id,
                            SubscriptionTime = DateTime.Now,
                        };

                        switch (pack.Feature)
                        {
                            case FeatureConstants.BUSINESS:
                                business.IsIndividual = false;
                                break;
                            case FeatureConstants.MATERIAL:
                                business.IsMaterialSupported = true;
                                break;
                            case FeatureConstants.SERVICE:
                                business.IsLimitServiceNum = false;
                                break;
                        }

                        await _packSubscriptionRepository.CreatePackSubscriptionAsync(newSubscription);
                    }

                    await _businessRepository.UpdateBusinessProfileAsync(business);

                    await _businessRepository.SaveAsync();
                });

                return new ApiResponse<VnPayResponse>("success", "Thanh Toán Gói Đăng Kí Thành Công!", null, 200);
            }
            else
            {
                if (result.VnPayResponseCode.Equals("11"))
                {
                    transaction.Status = StatusConstants.EXPIRED;
                }
                else
                {
                    transaction.Status = StatusConstants.FAILED;
                }

                await _transactionRepository.UpdateTransactionAsync(newTransaction);

                await _transactionRepository.SaveAsync();

                return new ApiResponse<VnPayResponse>("error", 400, "Thanh Toán Gói Đăng Kí Thất Bại!");
            }
        }
    }
}
