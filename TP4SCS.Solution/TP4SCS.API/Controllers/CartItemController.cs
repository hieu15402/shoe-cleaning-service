using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TP4SCS.Library.Models.Request.CartItem;
using TP4SCS.Library.Models.Response.AssetUrl;
using TP4SCS.Library.Models.Response.CartItem;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.Material;
using TP4SCS.Library.Utils.Utils;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;
        private readonly IServiceService _serviceService;
        private readonly IMaterialService _materialService;
        private readonly IMapper _mapper;

        public CartItemController(ICartItemService cartItemService, IServiceService serviceService, IMaterialService materialService, IMapper mapper)
        {
            _cartItemService = cartItemService;
            _serviceService = serviceService;
            _mapper = mapper;
            _materialService = materialService;
        }

        [HttpGet]
        [Route("api/cart/{id}/cartitems")]
        public async Task<IActionResult> GetCartItems(int id)
        {
            var items = await _cartItemService.GetCartItemsAsync(id);

            if (items == null || !items.Any())
            {
                return Ok(new ResponseObject<string>("Cart items retrieved successfully"));
            }
            List<CartItemResponse> itemsResponse = new List<CartItemResponse>();
            foreach (var item in items)
            {
                var service = await _serviceService.GetServiceByIdAsync(item.ServiceId!.Value);
                CartItemResponse cartItemResponse = new CartItemResponse
                {
                    Id = item.Id,
                    BranchId = item.BranchId,
                    ServiceId = item.ServiceId!.Value,
                    ServiceName = service!.Name,
                    ServiceStatus = service!.BranchServices.SingleOrDefault(bs => bs.BranchId == item.BranchId)!.Status,
                    Price = await _serviceService.GetServiceFinalPriceAsync(item.ServiceId!.Value),
                    CartId = item.CartId

                };
                if (!string.IsNullOrEmpty(item.MaterialIds))
                {
                    if (!string.IsNullOrEmpty(item.MaterialIds))
                    {
                        List<int> materialIds = Util.ConvertStringToList(item.MaterialIds);
                        var materials = await _materialService.GetMaterialsByIdsAsync(materialIds);
                        List<MaterialResponseV3> materialsResponse = new List<MaterialResponseV3>();
                        if (materials != null)
                        {
                            foreach (var m in materials)
                            {
                                materialsResponse.Add(new MaterialResponseV3
                                {
                                    Id = m.Id,
                                    AssetUrls = _mapper.Map<IEnumerable<AssetUrlResponse>>(m.AssetUrls).ToList(),
                                    BranchId = item.BranchId,
                                    Name = m.Name,
                                    Price = m.Price,
                                    Status = m.BranchMaterials.SingleOrDefault(bm => bm.BranchId == item.BranchId)!.Status,
                                    Storage = m.BranchMaterials.SingleOrDefault(bm => bm.BranchId == item.BranchId)!.Storage
                                });
                            }
                        }

                        cartItemResponse.Materials = materialsResponse;
                    }
                }
                itemsResponse.Add(cartItemResponse);
            }
            return Ok(new ResponseObject<List<CartItemResponse>>("Cart items retrieved successfully", itemsResponse));
        }

        [HttpGet]
        [Route("api/cartitems/{id}")]
        public async Task<IActionResult> GetCartItemById(int id)
        {
            var item = await _cartItemService.GetCartItemByIdAsync(id);
            if (item == null)
            {
                return NotFound(new ResponseObject<CartItemResponse>($"Mục có ID {id} không tìm thấy.", null));
            }
            var service = await _serviceService.GetServiceByIdAsync(item.ServiceId!.Value);
            CartItemResponse cartItemResponse = new CartItemResponse
            {
                Id = item.Id,
                BranchId = item.BranchId,
                ServiceId = item.ServiceId!.Value,
                ServiceName = service!.Name,
                ServiceStatus = service!.BranchServices.SingleOrDefault(bs => bs.BranchId == item.BranchId)!.Status,
                Price = await _serviceService.GetServiceFinalPriceAsync(item.ServiceId!.Value),
                CartId = item.CartId
            };
            if (!string.IsNullOrEmpty(item.MaterialIds))
            {
                if (!string.IsNullOrEmpty(item.MaterialIds))
                {
                    List<int> materialIds = Util.ConvertStringToList(item.MaterialIds);
                    var materials = await _materialService.GetMaterialsByIdsAsync(materialIds);
                    List<MaterialResponseV3> materialsResponse = new List<MaterialResponseV3>();
                    if (materials != null)
                    {
                        foreach (var m in materials)
                        {
                            materialsResponse.Add(new MaterialResponseV3
                            {
                                Id = m.Id,
                                AssetUrls = _mapper.Map<IEnumerable<AssetUrlResponse>>(m.AssetUrls).ToList(),
                                BranchId = item.BranchId,
                                Name = m.Name,
                                Price = m.Price,
                                Status = m.BranchMaterials.SingleOrDefault(bm => bm.BranchId == item.BranchId)!.Status,
                                Storage = m.BranchMaterials.SingleOrDefault(bm => bm.BranchId == item.BranchId)!.Storage
                            });
                        }
                    }

                    cartItemResponse.Materials = materialsResponse;
                }
            }
            return Ok(new ResponseObject<CartItemResponse>("Cart item retrieved successfully", cartItemResponse));
        }

        //[HttpGet]
        //[Route("api/cartitems/total")]
        //public async Task<IActionResult> CalculateCartItemsTotal([FromQuery] List<int> itemIds)
        //{
        //    try
        //    {
        //        decimal? total = await _cartItemService.CalculateCartItemsTotal(itemIds);

        //        return Ok(new ResponseObject<decimal?>("Tính toán thành công", total));
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(ex.Message);
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Đã xảy ra lỗi: {ex.Message}");
        //    }
        //}
        [HttpPost]
        [Route("api/cartitems")]
        public async Task<IActionResult> AddItemToCart([FromBody] CartItemCreateRequest request)
        {
            try
            {
                await _cartItemService.AddItemToCartAsync(request.AccountId, request.ServiceId, request.MaterialIds, request.BranchId);

                return Ok(new ResponseObject<string>("Add to cart success"));
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpDelete]
        [Route("api/cartitems")]
        public async Task<IActionResult> RemoveItemsFromCart([FromBody] List<int> itemIds)
        {
            try
            {
                if (itemIds == null || itemIds.Count == 0)
                {
                    return BadRequest("Danh sách ID của các mục không được để trống.");
                }

                await _cartItemService.RemoveItemsFromCartAsync(itemIds);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi máy chủ nội bộ: " + ex.Message);
            }
        }


    }
}
