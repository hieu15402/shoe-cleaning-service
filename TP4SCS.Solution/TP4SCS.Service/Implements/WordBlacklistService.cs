using ClosedXML.Excel;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using TP4SCS.Library.Models.Request.Feedback;
using TP4SCS.Library.Models.Request.WordBlacklist;
using TP4SCS.Library.Models.Response.Feedback;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class WordBlacklistService : IWordBlacklistService
    {
        private readonly string _filePath;

        public WordBlacklistService(string filePath)
        {
            _filePath = filePath;
        }

        public async Task<ApiResponse<WordBlacklistResponse>> AddWordAsync(UpdateWordBlacklistRequest wordBlacklistRequest)
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return new ApiResponse<WordBlacklistResponse>("error", 404, "Không Tìm Thấy File Từ Cấm!");
                }

                var json = await File.ReadAllTextAsync(_filePath);

                var allWords = JsonSerializer.Deserialize<List<WordBlacklistResponse>>(json) ?? new List<WordBlacklistResponse>();

                var isExisted = allWords
                        .Where(w => w.Word.Contains(wordBlacklistRequest.Word.Trim(), StringComparison.OrdinalIgnoreCase))
                        .Any();

                if (isExisted == true)
                {
                    return new ApiResponse<WordBlacklistResponse>("error", 400, "Từ Đã Tồn Tại Trong Danh Sách Cấm!");
                }

                var newWord = new WordBlacklistResponse
                {
                    Word = wordBlacklistRequest.Word.Trim(),
                    Note = wordBlacklistRequest.Note.Trim()
                };

                allWords.Add(newWord);

                json = JsonSerializer.Serialize(allWords, new JsonSerializerOptions { WriteIndented = true });

                await File.WriteAllTextAsync(_filePath, json, Encoding.UTF8);

                return new ApiResponse<WordBlacklistResponse>("success", "Cập Nhập Danh Sách Từ Cấm Thành Công!", newWord, 201);
            }
            catch (Exception)
            {
                return new ApiResponse<WordBlacklistResponse>("error", 400, "Cập Nhập Danh Sách Từ Cấm Thất Bại!");
            }
        }

        public async Task<ApiResponse<WordBlacklistResponse>> DeleteWordAsync(DeleteWordBlacklistRequest wordBlacklistRequest)
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return new ApiResponse<WordBlacklistResponse>("error", 404, "Không Tìm Thấy File Từ Cấm!");
                }

                var json = await File.ReadAllTextAsync(_filePath);
                var allWords = JsonSerializer.Deserialize<List<WordBlacklistResponse>>(json) ?? new List<WordBlacklistResponse>();

                var wordToRemove = allWords.FirstOrDefault(w => w.Word.Equals(wordBlacklistRequest.Word.Trim(), StringComparison.OrdinalIgnoreCase));

                if (wordToRemove == null)
                {
                    return new ApiResponse<WordBlacklistResponse>("error", 404, "Không Tìm Thấy Từ Cấm!");
                }

                allWords.Remove(wordToRemove);

                json = JsonSerializer.Serialize(allWords, new JsonSerializerOptions { WriteIndented = true });

                await File.WriteAllTextAsync(_filePath, json, Encoding.UTF8);

                return new ApiResponse<WordBlacklistResponse>("error", "Xoá Từ Cấm Thành Công!", null);
            }
            catch (Exception)
            {
                return new ApiResponse<WordBlacklistResponse>("error", 400, "Xoá Từ Cấm Thất Bại!");
            }
        }

        public async Task<ApiResponse<byte[]>> ExportAsExcelAsync()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return new ApiResponse<byte[]>("error", 404, "Không Tìm Thấy File Từ Cấm!");
                }

                var json = await File.ReadAllTextAsync(_filePath);

                var allWords = JsonSerializer.Deserialize<List<WordBlacklistResponse>>(json) ?? new List<WordBlacklistResponse>();

                using var workbook = new XLWorkbook();

                var worksheet = workbook.Worksheets.Add("Word Blacklist");
                worksheet.Cell(1, 1).Value = "Word";
                worksheet.Cell(1, 2).Value = "Note";

                for (int i = 0; i < allWords.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = allWords[i].Word;
                    worksheet.Cell(i + 2, 2).Value = allWords[i].Note;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();

                workbook.SaveAs(stream);

                var data = stream.ToArray();

                return new ApiResponse<byte[]>("success", "Tải Danh Sách Từ Cấm Thành Công!", data);
            }
            catch (Exception)
            {
                return new ApiResponse<byte[]>("error", 400, "Tải Danh Sách Từ Cấm Thất Bại!");
            }
        }

        public async Task<ApiResponse<IEnumerable<WordBlacklistResponse>?>> GetBlacklistAsync(GetWordBlacklistRequest getWordBlacklistRequest)
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return new ApiResponse<IEnumerable<WordBlacklistResponse>?>("error", 404, "Không Tìm Thấy File Từ Cấm!");
                }

                var json = await File.ReadAllTextAsync(_filePath);

                var words = JsonSerializer.Deserialize<List<WordBlacklistResponse>>(json) ?? new List<WordBlacklistResponse>();

                int totalWord = words.Count;

                int totalPage = (int)Math.Ceiling((decimal)totalWord / getWordBlacklistRequest.PageSize);

                if (!string.IsNullOrEmpty(getWordBlacklistRequest.SearchKey))
                {
                    words = words
                        .Where(w => w.Word.Contains(getWordBlacklistRequest.SearchKey.Trim(), StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (words.IsNullOrEmpty())
                    {
                        return new ApiResponse<IEnumerable<WordBlacklistResponse>?>("error", 404, "Không Tìm Thấy Từ Cấm!");
                    }
                }

                var pagination = new Pagination(totalWord, getWordBlacklistRequest.PageSize, getWordBlacklistRequest.PageNum, totalPage);

                var data = words
                    .Skip((getWordBlacklistRequest.PageNum - 1) * getWordBlacklistRequest.PageSize)
                    .Take(getWordBlacklistRequest.PageSize);

                return new ApiResponse<IEnumerable<WordBlacklistResponse>?>("success", "Lấy Thông Tin Danh Sách Từ Cấm Thành Công!", data, 200, pagination);
            }
            catch (Exception)
            {
                return new ApiResponse<IEnumerable<WordBlacklistResponse>?>("error", 400, "Lấy Thông Tin Danh Sách Từ Cấm Thất Bại!");
            }
        }

        public async Task<ApiResponse<WordBlacklistResponse>> ImportWordBlacklistAsync(Stream excelFileStream)
        {
            try
            {
                using var workbook = new XLWorkbook(excelFileStream);
                var worksheet = workbook.Worksheet(1);

                var headerWord = worksheet.Cell(1, 1).GetValue<string>().Trim();
                var headerType = worksheet.Cell(1, 2).GetValue<string>().Trim();

                if (headerWord != "Word" || headerType != "Note")
                {
                    return new ApiResponse<WordBlacklistResponse>("error", 400, "File Danh Sách Từ Cấm Không Hợp Lệ!");
                }

                var wordBlacklist = new List<WordBlacklistResponse>();
                var row = 2;

                while (!worksheet.Cell(row, 1).IsEmpty())
                {
                    var word = worksheet.Cell(row, 1).GetValue<string>().Trim();
                    var type = worksheet.Cell(row, 2).GetValue<string>().Trim();

                    if (string.IsNullOrEmpty(word) || string.IsNullOrEmpty(type))
                    {
                        return new ApiResponse<WordBlacklistResponse>("error", 400, $"Dữ Liệu Ở Dòng {row} Trống!");
                    }

                    wordBlacklist.Add(new WordBlacklistResponse { Word = word, Note = type });
                    row++;
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                var json = JsonSerializer.Serialize(wordBlacklist, options);

                if (File.Exists(_filePath))
                {
                    await File.WriteAllTextAsync(_filePath, "[]", Encoding.UTF8);
                }

                await File.WriteAllTextAsync(_filePath, json, Encoding.UTF8);

                return new ApiResponse<WordBlacklistResponse>("success", 201, "Tạo Danh Sách Từ Cấm Mới Thành Công!");
            }
            catch (Exception ex)
            {
                return new ApiResponse<WordBlacklistResponse>("error", 500, $"Tạo Danh Sách Từ Cấm Mới Thất Bại! Lỗi: {ex.Message}");
            }
        }

    }
}
