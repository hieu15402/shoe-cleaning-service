using TP4SCS.Library.Models.Request.Feedback;
using TP4SCS.Library.Models.Request.WordBlacklist;
using TP4SCS.Library.Models.Response.Feedback;
using TP4SCS.Library.Models.Response.General;

namespace TP4SCS.Services.Interfaces
{
    public interface IWordBlacklistService
    {
        Task<ApiResponse<IEnumerable<WordBlacklistResponse>?>> GetBlacklistAsync(GetWordBlacklistRequest getWordBlacklistRequest);

        Task<ApiResponse<WordBlacklistResponse>> AddWordAsync(UpdateWordBlacklistRequest wordBlacklistRequest);

        Task<ApiResponse<WordBlacklistResponse>> DeleteWordAsync(DeleteWordBlacklistRequest wordBlacklistRequest);

        Task<ApiResponse<WordBlacklistResponse>> ImportWordBlacklistAsync(Stream excelFileStream);

        Task<ApiResponse<byte[]>> ExportAsExcelAsync();
    }
}
