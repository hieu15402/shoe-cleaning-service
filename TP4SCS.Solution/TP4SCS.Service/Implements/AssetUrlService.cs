using Microsoft.AspNetCore.Http;
using System.Net;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.AssetUrl;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class AssetUrlService : IAssetUrlService
    {
        //private readonly string _bucketName = "whalehome-project.appspot.com";
        //private readonly StorageClient _storageClient;
        private IAssetUrlRepository _assetUrlRepository;
        public AssetUrlService(IAssetUrlRepository assetUrlRepository)
        {
            //var credentialPath = Path.Combine(Directory.GetCurrentDirectory(), "firebase.json");
            //GoogleCredential credential = GoogleCredential.FromFile(credentialPath);
            //_storageClient = StorageClient.Create(credential);


            _assetUrlRepository = assetUrlRepository;
        }

        public async Task<AssetUrl?> GetAssestUrlByIdAsync(int id)
        {
            return await _assetUrlRepository.GetAssetUrlByIdAsync(id);
        }

        public async Task<List<AssetUrl>> AddAssestUrlsAsync(
            List<AssetUrlRequest> assetUrlRequests,
            int? businessId = null,
            int? feedbackId = null,
            int? serviceId = null)
        {
            List<AssetUrl> assetUrls = new List<AssetUrl>();
            foreach (var assetUrlRequest in assetUrlRequests)
            {

                var assetUrl = new AssetUrl
                {
                    BusinessId = businessId,
                    FeedbackId = feedbackId,
                    ServiceId = serviceId,
                    Url = assetUrlRequest.Url,
                    //IsImage = assetUrlRequest.IsImage,
                    Type = assetUrlRequest.Type
                };

                assetUrls.Add(assetUrl);
            }
            await _assetUrlRepository.AddAssetUrlsAsync(assetUrls);
            return assetUrls;
        }
        public async Task DeleteAssetUrlsAsync(int id)
        {
            await _assetUrlRepository.DeleteAssetUrlAsync(id);
        }
        public async Task UpdateAssetUrlsAsync(List<AssetUrl> assetUrls)
        {
            await _assetUrlRepository.UpdateAssetUrlsAsync(assetUrls);
        }
        public async Task DeleteAssetUrlAsync(int assetUrlId)
        {
            var assetUrl = await _assetUrlRepository.GetAssetUrlByIdAsync(assetUrlId);
            if (assetUrl == null)
            {
                throw new Exception("Không tìm thấy AssetUrl.");
            }
            await _assetUrlRepository.DeleteAssetUrlAsync(assetUrlId);
        }




        // Phương thức upload ảnh
        //public async Task<FileResponse> UploadFileAsync(IFormFile file)
        //{
        //    try
        //    {
        //        string fileName = GenerateFileName(file.FileName);
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            await file.CopyToAsync(memoryStream);
        //            memoryStream.Position = 0;

        //            var storageObject = await _storageClient.UploadObjectAsync(
        //                bucket: _bucketName,
        //                objectName: fileName,
        //                contentType: file.ContentType,
        //                source: memoryStream);
        //            var fileResponse = new FileResponse()
        //            {
        //                IsImage = IsImage(file),
        //                Type = file.ContentType,
        //                Url = $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{WebUtility.UrlEncode(fileName)}?alt=media"
        //        };
        //            return fileResponse;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Lỗi upload hình ảnh: {ex.Message}");
        //    }
        //}
        //public async Task DeleteImageAsync(string fileUrl)
        //{
        //    var exist = await CheckImageExistsAsync(fileUrl);
        //    if (!exist) return;
        //    var fileName = ExtractFileNameFromUrl(fileUrl);
        //    if (string.IsNullOrEmpty(fileName))
        //    {
        //        throw new Exception("URL không hợp lệ.");
        //    }
        //    await _storageClient.DeleteObjectAsync(_bucketName, fileName);
        //}

        private async Task<bool> CheckImageExistsAsync(string imageUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Head, imageUrl);
                var response = await client.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
        }

        // Phương thức trích xuất tên tệp từ URL
        private string? ExtractFileNameFromUrl(string fileUrl)
        {
            // Phân tích URL để lấy tên tệp
            var uri = new Uri(fileUrl);
            var segments = uri.Segments;
            if (segments.Length < 3)
            {
                return null; // Nếu không đủ thông tin để lấy tên tệp
            }

            // Lấy phần tên tệp từ segments
            var fileName = WebUtility.UrlDecode(segments[segments.Length - 1]); // Phần cuối cùng của URL là tên tệp
            return fileName.Split('?')[0]; // Xóa tham số truy vấn nếu có
        }
        private string GenerateFileName(string originalFileName)
        {
            var fileExtension = Path.GetExtension(originalFileName);
            return $"{Guid.NewGuid()}{fileExtension}";
        }
        private bool IsImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return false;
            }

            return file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
        }

    }
}
