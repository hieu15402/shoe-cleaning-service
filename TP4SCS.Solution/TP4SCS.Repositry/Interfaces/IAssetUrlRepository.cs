using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;

namespace TP4SCS.Repository.Interfaces
{
    public interface IAssetUrlRepository
    {
        Task<AssetUrl?> GetAssetUrlByIdAsync(int id);
        Task<IEnumerable<AssetUrl>?> GetAssetUrlsAsync(
            int? pageIndex = null,
            int? pageSize = null,
            OrderByEnum orderBy = OrderByEnum.IdAsc);
        Task AddAssetUrlsAsync(List<AssetUrl> assetUrls);
        Task UpdateAssetUrlAsync(AssetUrl assetUrl);
        Task DeleteAssetUrlAsync(int id);
        Task UpdateAssetUrlsAsync(List<AssetUrl> assetUrls);
    }
}
