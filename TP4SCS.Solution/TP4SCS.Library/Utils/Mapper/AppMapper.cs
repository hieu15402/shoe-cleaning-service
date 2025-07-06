using AutoMapper;
using Mapster;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.AssetUrl;
using TP4SCS.Library.Models.Request.CartItem;
using TP4SCS.Library.Models.Request.Category;
using TP4SCS.Library.Models.Request.Feedback;
using TP4SCS.Library.Models.Request.Material;
using TP4SCS.Library.Models.Request.OrderDetail;
using TP4SCS.Library.Models.Request.Process;
using TP4SCS.Library.Models.Request.Promotion;
using TP4SCS.Library.Models.Request.Service;
using TP4SCS.Library.Models.Response.AssetUrl;
using TP4SCS.Library.Models.Response.Branch;
using TP4SCS.Library.Models.Response.BranchMaterial;
using TP4SCS.Library.Models.Response.BranchService;
using TP4SCS.Library.Models.Response.Cart;
using TP4SCS.Library.Models.Response.CartItem;
using TP4SCS.Library.Models.Response.Category;
using TP4SCS.Library.Models.Response.Feedback;
using TP4SCS.Library.Models.Response.Material;
using TP4SCS.Library.Models.Response.Order;
using TP4SCS.Library.Models.Response.OrderDetail;
using TP4SCS.Library.Models.Response.Process;
using TP4SCS.Library.Models.Response.Promotion;
using TP4SCS.Library.Models.Response.Service;
using TP4SCS.Library.Utils.Utils;

namespace TP4SCS.Library.Utils.Mapper
{
    public class AppMapper : Profile, IRegister
    {
        public AppMapper()
        {
            //Service Mapping
            CreateMap<Service, ServiceResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Util.TranslateGeneralStatus(src.Status)));
            CreateMap<Service, ServiceResponseV3>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Util.TranslateGeneralStatus(src.Status)))
                .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src => src.BranchServices.FirstOrDefault()!.Branch.Business.Name))
                .ForMember(dest => dest.BusinessId, opt => opt.MapFrom(src => src.BranchServices.FirstOrDefault()!.Branch.BusinessId));
            CreateMap<Service, ServiceCreateResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Util.TranslateGeneralStatus(src.Status)));
            CreateMap<Service, ServiceResponseV2>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Util.TranslateGeneralStatus(src.Status)))
            .ForMember(dest => dest.AssetUrls, opt => opt.MapFrom(src => src.AssetUrls));
            CreateMap<ServiceRequest, Service>();
            CreateMap<ServiceCreateRequest, Service>();

            // Category Mappings
            CreateMap<ServiceCategory, ServiceCategoryResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Util.TranslateGeneralStatus(src.Status)));
            CreateMap<ServiceCategoryRequest, ServiceCategory>();

            //Cart Mappings
            CreateMap<Cart, CartResponse>();

            //Cart Item Mappings
            CreateMap<CartItemCreateRequest, CartItem>()
                .ForMember(dest => dest.MaterialIds, opt => opt.MapFrom(src => Util.ConvertListToString(src.MaterialIds)));

            CreateMap<CartItem, CartItemResponse>();

            //Promotion Mappings
            CreateMap<Promotion, PromotionResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Util.TranslateGeneralStatus(src.Status)));
            CreateMap<PromotionCreateRequest, Promotion>();
            CreateMap<PromotionUpdateRequest, Promotion>();

            //Material Mappings
            CreateMap<MaterialCreateRequest, Material>();
            CreateMap<MaterialUpdateRequest, Material>();
            CreateMap<Material, MaterialResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Util.TranslateGeneralStatus(src.Status)));
            CreateMap<Material, MaterialResponseV2>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Util.TranslateGeneralStatus(src.Status)))
                .ForMember(dest => dest.AssetUrls, opt => opt.MapFrom(src => src.AssetUrls));
            CreateMap<Material, MaterialResponseV3>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Util.TranslateGeneralStatus(src.Status)))
                .ForMember(dest => dest.AssetUrls, opt => opt.MapFrom(src => src.AssetUrls));
            // Configure AutoMapper

            //AssetUrl Mapping
            CreateMap<AssetUrl, AssetUrlResponse>();
            CreateMap<AssetUrlRequest, AssetUrl>();

            //BranchService Mapping
            CreateMap<BranchService, BranchServiceResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Util.TranslateGeneralStatus(src.Status)));
            //Process Mapping
            CreateMap<ServiceProcess, ProcessResponse>();
            CreateMap<ProcessCreateRequest, ServiceProcess>();
            CreateMap<ProcessUpdateRequest, ServiceProcess>();

            //BranchMaterial Mapping
            CreateMap<BranchMaterial, BranchMaterialResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Util.TranslateGeneralStatus(src.Status)));

            //BusinessBranch Mapping
            CreateMap<BusinessBranch, BranchResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Util.TranslateBranchStatus(src.Status)));

            //Order Mapping
            CreateMap<Order, OrderResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Util.TranslateOrderStatus(src.Status)))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Account.Phone))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Account.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Account.Email));
            CreateMap<Order, OrderFeedbackResponse>();

            //Feedback Mapping
            CreateMap<FeedbackRequest, Feedback>();
            CreateMap<FeedbackUpdateRequest, Feedback>();
            CreateMap<FeedbackUpdateRequestV2, Feedback>();
            CreateMap<Feedback, FeedbackResponse>()
                .ForMember(dest => dest.OrderItem, opt => opt.MapFrom(src => src.OrderItem)) // OrderItem ánh xạ trực tiếp
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.OrderItem.Order)) // Order ánh xạ qua OrderItem
                .ForMember(dest => dest.AssetUrls, opt => opt.MapFrom(src => src.AssetUrls));
            CreateMap<Feedback, FeedbackResponseForAdmin>();

            //OrderDetail Mapping
            CreateMap<OrderDetailUpdateRequest, OrderDetail>();
            CreateMap<OrderDetail, OrderDetailResponse>();
            CreateMap<OrderDetail, OrderDetailResponseV2>();
            CreateMap<OrderDetail, OrderDetailResponseV3>()
                .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => src.Branch))
                .ForMember(dest => dest.Service, opt => opt.MapFrom(src => src.Service));
        }

        public void Register(TypeAdapterConfig config)
        {

        }
    }
}
