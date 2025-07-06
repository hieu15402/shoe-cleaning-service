using Microsoft.AspNetCore.Mvc;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetUrlController : ControllerBase
    {
        private readonly IAssetUrlService _assetUrlService;
        public AssetUrlController(IAssetUrlService assetUrlService)
        {
            _assetUrlService = assetUrlService;
        }
    }
}
