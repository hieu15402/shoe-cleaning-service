using Mapster;
using MapsterMapper;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Category;
using TP4SCS.Library.Models.Response.Category;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Library.Utils.Utils;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class TicketCategoryService : ITicketCategoryService
    {
        private readonly ITicketCategoryRepository _ticketCategoryRepository;
        private readonly IMapper _mapper;
        private readonly Util _util;

        public TicketCategoryService(ITicketCategoryRepository ticketCategoryRepository, IMapper mapper, Util util)
        {
            _ticketCategoryRepository = ticketCategoryRepository;
            _mapper = mapper;
            _util = util;
        }

        //Create Ticket Category
        public async Task<ApiResponse<TicketCategoryResponse>> CreateCategoryAsync(TicketCategoryRequest ticketCategoryRequest)
        {
            var isNameExisted = await _ticketCategoryRepository.IsNameExistedAsync(_util.FormatStringName(ticketCategoryRequest.Name));

            if (isNameExisted)
            {
                return new ApiResponse<TicketCategoryResponse>("error", 400, "Tên Loại Phiếu Đã Tồn Tại!");
            }

            var newCategory = _mapper.Map<TicketCategory>(ticketCategoryRequest);
            newCategory.Name = _util.FormatStringName(newCategory.Name);

            try
            {
                await _ticketCategoryRepository.CreateCategoryAsync(newCategory);

                var newTck = await GetCategoryByIdAsync(newCategory.Id);

                return new ApiResponse<TicketCategoryResponse>("success", "Tạo Loại Phiếu Mới Thành Công!", newTck.Data, 201);
            }
            catch (Exception)
            {
                return new ApiResponse<TicketCategoryResponse>("error", 400, "Tạo Loại Phiếu Mới Thất Bại!");
            }
        }

        //Get Ticket Categories
        public async Task<ApiResponse<IEnumerable<TicketCategoryResponse>?>> GetCategoriesAsync()
        {
            var categories = await _ticketCategoryRepository.GetCategoriesAsync();

            if (categories == null)
            {
                return new ApiResponse<IEnumerable<TicketCategoryResponse>?>("error", 404, "Loại Phiếu Trống!");
            }

            var data = categories.Adapt<IEnumerable<TicketCategoryResponse>>();

            return new ApiResponse<IEnumerable<TicketCategoryResponse>?>("success", "Lấy Thông Tin Loại Phiếu Thành Công!", data, 200);
        }

        //Get Ticket Category By Id
        public async Task<ApiResponse<TicketCategoryResponse?>> GetCategoryByIdAsync(int id)
        {

            var category = await _ticketCategoryRepository.GetCategoryByIdAsync(id);

            if (category == null)
            {
                return new ApiResponse<TicketCategoryResponse?>("error", 404, "Không Tìm Thấy Loại Phiếu!");
            }

            var data = _mapper.Map<TicketCategoryResponse>(category);

            return new ApiResponse<TicketCategoryResponse?>("success", "Lấy Thông Tin Loại Phiếu Thành Công!", data, 200);
        }

        //Update Ticket Category
        public async Task<ApiResponse<TicketCategoryResponse>> UpdateCategoryAsync(int id, UpdateTicketCategoryRequest updateTicketCategoryRequest)
        {
            var oldCategory = await _ticketCategoryRepository.GetCategoryByIdAsync(id);

            if (oldCategory == null)
            {
                return new ApiResponse<TicketCategoryResponse>("error", 404, "Không Tìm Thấy Loại Phiếu!");
            }

            var newName = _util.FormatStringName(updateTicketCategoryRequest.Name);

            var isNameExisted = await _ticketCategoryRepository.IsNameExistedAsync(newName);

            if (isNameExisted && newName.Equals(oldCategory.Name, StringComparison.CurrentCultureIgnoreCase) == false)
            {
                return new ApiResponse<TicketCategoryResponse>("error", 400, "Tên Loại Phiếu Đã Tồn Tại!");
            }

            var newCategory = _mapper.Map(updateTicketCategoryRequest, oldCategory);
            newCategory.Status = updateTicketCategoryRequest.Status switch
            {
                TicketCateStatus.AVAILABLE => StatusConstants.AVAILABLE,
                TicketCateStatus.UNAVAILABLE => StatusConstants.UNAVAILABLE,
                _ => StatusConstants.AVAILABLE
            };

            try
            {
                await _ticketCategoryRepository.UpdateAsync(newCategory);

                return new ApiResponse<TicketCategoryResponse>("success", "Cập Nhập Loại Phiếu Thành Công!", null, 200);
            }
            catch (Exception)
            {
                return new ApiResponse<TicketCategoryResponse>("error", 400, "Cập Nhập Loại Phiếu Thất Bại!");
            }
        }
    }
}
