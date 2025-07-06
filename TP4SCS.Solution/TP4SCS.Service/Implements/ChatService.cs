using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using TP4SCS.Library.Models.Request.Chat;
using TP4SCS.Library.Models.Response.Chat;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class ChatService : IChatService
    {
        //private readonly string _firebaseDbUrl = "https://tp4scs-default-rtdb.asia-southeast1.firebasedatabase.app";
        private readonly string _firebaseDbUrl = "https://shoecarehub-4dca3-default-rtdb.asia-southeast1.firebasedatabase.app/";
        private readonly IAccountRepository _accountRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IHttpClientFactory _httpClientFactory;

        public ChatService(IConfiguration configuration,
            IAccountRepository accountRepository,
            IBusinessRepository businessRepository,
            IHttpClientFactory httpClientFactory)
        {
            var firebaseFilePath = configuration["Firebase:FilePath"];

            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(firebaseFilePath)
                });
            }

            _accountRepository = accountRepository;
            _businessRepository = businessRepository;
            _httpClientFactory = httpClientFactory;
        }

        private async Task AddToFirebaseAsync<T>(string path, T data)
        {
            using var httpClient = _httpClientFactory.CreateClient();

            var json = JsonConvert.SerializeObject(data);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await httpClient.PutAsync($"{_firebaseDbUrl}/{path}.json", content);
        }

        private async Task<T?> GetFromFirebaseAsync<T>(string path)
        {
            using var httpClient = _httpClientFactory.CreateClient();

            var response = await httpClient.GetStringAsync($"{_firebaseDbUrl}/{path}.json");

            return JsonConvert.DeserializeObject<T>(response);
        }

        private async Task DeleteFromFirebaseAsync(string path)
        {
            using var httpClient = _httpClientFactory.CreateClient();

            await httpClient.DeleteAsync($"{_firebaseDbUrl}/{path}.json");
        }

        public async Task<ApiResponse<ChatRoomResponse>> CreateChatRoomAsync(ChatRoomRequest roomRequest)
        {
            var existingRoom1 = await GetChatRoomAsync(roomRequest.AccountId1, roomRequest.AccountId2);

            var existingRoom2 = await GetChatRoomAsync(roomRequest.AccountId1, roomRequest.AccountId2);

            if (existingRoom1 != null || existingRoom2 != null)
            {
                return new ApiResponse<ChatRoomResponse>("error", "Phòng Chat Đã Tồn Tại!", existingRoom1, 400);
            }

            var acc1 = await _accountRepository.GetAccountByIdNoTrackingAsync(roomRequest.AccountId1);
            var acc2 = await _accountRepository.GetAccountByIdNoTrackingAsync(roomRequest.AccountId2);

            if (acc1 == null || acc2 == null)
            {
                return new ApiResponse<ChatRoomResponse>("error", 404, "Không Tìm Thấy Thông Tin Người Dùng!");
            }

            var acc1Name = acc1.FullName;
            var acc1Url = acc1.ImageUrl;

            var acc2Name = acc2.FullName;
            var acc2Url = acc2.ImageUrl;

            if (acc1.Role.Equals(RoleConstants.OWNER))
            {
                var business = await _businessRepository.GetBusinessByOwnerIdNoTrackingAsync(roomRequest.AccountId1);

                if (business == null)
                {
                    acc1Name = acc1.FullName;
                    acc1Url = acc1.ImageUrl;
                }
                else
                {
                    acc1Name = business.Name;
                    acc1Url = business.ImageUrl;
                }
            }

            if (acc2.Role.Equals(RoleConstants.OWNER))
            {
                var business = await _businessRepository.GetBusinessByOwnerIdNoTrackingAsync(roomRequest.AccountId2);

                if (business == null)
                {
                    acc2Name = acc2.FullName;
                    acc2Url = acc2.ImageUrl;
                }
                else
                {
                    acc2Name = business.Name;
                    acc2Url = business.ImageUrl;
                }
            }

            var room = new ChatRoomResponse
            {
                Id = $"{roomRequest.AccountId1}_{roomRequest.AccountId2}",
                AccountId1 = roomRequest.AccountId1,
                Account1FullName = acc1Name,
                Account1ImageUrl = acc1Url ?? "",
                IsAccount1Seen = true,
                AccountId2 = roomRequest.AccountId2,
                Account2FullName = acc2Name,
                Account2ImageUrl = acc2Url ?? "",
                IsAccount2Seen = true
            };

            try
            {
                await AddToFirebaseAsync($"chatRooms/{room.Id}", room);

                return new ApiResponse<ChatRoomResponse>("success", "Tạo Phòng Chat Thành Công!", room);
            }
            catch (Exception)
            {
                return new ApiResponse<ChatRoomResponse>("error", 400, "Tạo Phòng Chat Thất Bại!");
            }

        }

        private async Task<ChatRoomResponse?> GetChatRoomAsync(int accId1, int accId2)
        {
            return await GetFromFirebaseAsync<ChatRoomResponse>($"chatRooms/{accId1}_{accId2}");
        }

        public async Task<ApiResponse<IEnumerable<ChatRoomResponse>?>> GetChatRoomsAsync(int accId)
        {
            try
            {
                var chatRooms = await GetFromFirebaseAsync<Dictionary<string, ChatRoomResponse>>("chatRooms");

                if (chatRooms == null)
                {
                    return new ApiResponse<IEnumerable<ChatRoomResponse>?>("error", 404, "Thông Tin Phòng Chat Trống!");
                };

                var filteredRooms = chatRooms
                    .Where(cr => cr.Key.Split('_').Contains(accId.ToString()))
                    .Select(cr => cr.Value)
                    .Distinct()
                    .ToList();

                if (filteredRooms == null)
                {
                    return new ApiResponse<IEnumerable<ChatRoomResponse>?>("error", 404, "Không Tìm Thấy Thông Tin Phòng Chat!");
                }

                return new ApiResponse<IEnumerable<ChatRoomResponse>?>("success", "Lấy Thông Tin Phòng Chat Thành Công!", filteredRooms);
            }
            catch (Exception)
            {
                return new ApiResponse<IEnumerable<ChatRoomResponse>?>("error", 400, "Lấy Thông Tin Phòng Chat Thất Bại!");
            }
        }

        public async Task<ApiResponse<IEnumerable<MessageResponse>?>> GetMessagesAsync(int id, string roomId)
        {
            try
            {
                var room = await GetFromFirebaseAsync<ChatRoomResponse>($"chatRooms/{roomId}");

                if (room == null)
                {
                    return new ApiResponse<IEnumerable<MessageResponse>?>("error", 404, "Không Tìm Thấy Phòng Chat!");
                }

                if (room.AccountId1 == id)
                {
                    room.IsAccount1Seen = true;
                }
                else
                {
                    room.IsAccount2Seen = true;
                }

                await AddToFirebaseAsync($"chatRooms/{roomId}", room);

                var messages = await GetFromFirebaseAsync<Dictionary<string, MessageResponse>>($"messages/{roomId}");

                if (messages == null)
                {
                    return new ApiResponse<IEnumerable<MessageResponse>?>("error", 404, "Thông Tin Chat Trống!");
                }

                var result = messages?.Values.OrderBy(m => m.Timestamp) ?? Enumerable.Empty<MessageResponse>();

                return new ApiResponse<IEnumerable<MessageResponse>?>("success", "Lấy Thông Tin Chat Thành Công!", result);
            }
            catch (Exception)
            {
                return new ApiResponse<IEnumerable<MessageResponse>?>("error", 400, "Lấy Thông Tin Chat Thất Bại!");
            }
        }

        public async Task<ApiResponse<MessageResponse>> SendMessageAsync(int id, MessageRequest messageRequest)
        {
            var room = await GetFromFirebaseAsync<ChatRoomResponse>($"chatRooms/{messageRequest.RoomId}");

            if (room == null)
            {
                return new ApiResponse<MessageResponse>("error", 404, "Không Tìm Thấy Thông Tin Chat!");
            }

            if (room.AccountId1 == id)
            {
                room.IsAccount1Seen = true;
                room.IsAccount2Seen = false;
            }
            else
            {
                room.IsAccount1Seen = false;
                room.IsAccount2Seen = true;
            }

            if (!string.IsNullOrEmpty(messageRequest.Content) && messageRequest.IsImage == true)
            {
                return new ApiResponse<MessageResponse>("error", 400, "Message Đang Là Nội Dung Ảnh!");
            }


            if (messageRequest.ImageUrls!.Count > 0 && messageRequest.IsImage == false)
            {
                return new ApiResponse<MessageResponse>("error", 400, "Message Đang Là Nội Dung Chữ!");
            }

            var account = await _accountRepository.GetAccountByIdNoTrackingAsync(messageRequest.SenderId);

            if (account == null)
            {
                return new ApiResponse<MessageResponse>("error", 404, "Không Tìm Thấy Thông Tin Tài Khoản");
            }

            var fullName = account.FullName;
            var imageUrl = account.ImageUrl;
            var isOwner = false;

            if (account.Role.Equals(RoleConstants.OWNER))
            {
                var business = await _businessRepository.GetBusinessByOwnerIdNoTrackingAsync(account.Id);

                if (business == null)
                {
                    return new ApiResponse<MessageResponse>("error", 404, "Không Tìm Thấy Thông Tin Doanh Nghiệp");
                }

                fullName = business.Name;
                imageUrl = business.ImageUrl;
                isOwner = true;
            }

            var message = new MessageResponse
            {
                Id = Guid.NewGuid().ToString(),
                RoomId = messageRequest.RoomId,
                SenderId = account.Id,
                FullName = fullName,
                ImageUrl = imageUrl!,
                IsImage = messageRequest.IsImage,
                IsOrder = false,
                IsOwner = isOwner,
                Timestamp = DateTime.Now,
            };

            if (messageRequest.IsImage == false)
            {
                message.Content = messageRequest.Content;
                message.ImageUrls = null;
            }
            else
            {
                message.Content = null;
                message.ImageUrls = message.ImageUrls;
            }

            try
            {
                await AddToFirebaseAsync($"chatRooms/{messageRequest.RoomId}", room);

                await AddToFirebaseAsync($"messages/{message.RoomId}/{message.Id}", message);

                return new ApiResponse<MessageResponse>("success", "Gửi Tin Nhắn Thành Công!", message, 201);
            }
            catch (Exception)
            {
                return new ApiResponse<MessageResponse>("error", 400, "Gửi Tin Nhắn Thất Bại!");
            }
        }

        public async Task<ApiResponse<MessageResponse>> SendOrderMessageAsync(int id, OrderMessageRequest orderMessageRequest)
        {
            var room = await GetFromFirebaseAsync<ChatRoomResponse>($"chatRooms/{orderMessageRequest.RoomId}");

            if (room == null)
            {
                return new ApiResponse<MessageResponse>("error", 404, "Không Tìm Thấy Thông Tin Chat!");
            }

            if (room.AccountId1 == id)
            {
                room.IsAccount1Seen = true;
                room.IsAccount2Seen = false;
            }
            else
            {
                room.IsAccount1Seen = false;
                room.IsAccount2Seen = true;
            }

            var account = await _accountRepository.GetAccountByIdNoTrackingAsync(orderMessageRequest.SenderId);

            if (account == null)
            {
                return new ApiResponse<MessageResponse>("error", 404, "Không Tìm Thấy Thông Tin Tài Khoản");
            }

            var fullName = account.FullName;
            var imageUrl = account.ImageUrl;
            var isOwner = false;

            if (account.Role.Equals(RoleConstants.OWNER))
            {
                var business = await _businessRepository.GetBusinessByOwnerIdNoTrackingAsync(account.Id);

                if (business == null)
                {
                    return new ApiResponse<MessageResponse>("error", 404, "Không Tìm Thấy Thông Tin Doanh Nghiệp");
                }

                fullName = business.Name;
                imageUrl = business.ImageUrl;
                isOwner = true;
            }

            var message = new MessageResponse
            {
                Id = Guid.NewGuid().ToString(),
                RoomId = orderMessageRequest.RoomId,
                SenderId = account.Id,
                FullName = fullName,
                ImageUrl = imageUrl!,
                Content = orderMessageRequest.OrderId.ToString(),
                ImageUrls = null,
                IsImage = false,
                IsOrder = true,
                IsOwner = isOwner,
                Timestamp = DateTime.Now,
            };

            try
            {
                await AddToFirebaseAsync($"chatRooms/{orderMessageRequest.RoomId}", room);

                await AddToFirebaseAsync($"messages/{message.RoomId}/{message.Id}", message);

                return new ApiResponse<MessageResponse>("success", "Gửi Tin Nhắn Thành Công!", message, 201);
            }
            catch (Exception)
            {
                return new ApiResponse<MessageResponse>("error", 400, "Gửi Tin Nhắn Thất Bại!");
            }
        }

        public async Task<ApiResponse<string>> DeleteChatRoomAsync(string roomId)
        {
            try
            {
                var room = await GetFromFirebaseAsync<ChatRoomResponse>($"chatRooms/{roomId}");

                if (room == null)
                {
                    return new ApiResponse<string>("error", 404, "Không Tìm Thấy Phòng Chat!");
                }

                await DeleteFromFirebaseAsync($"messages/{roomId}");

                await DeleteFromFirebaseAsync($"chatRooms/{roomId}");

                return new ApiResponse<string>("success", "Xóa Phòng Chat Thành Công!", roomId);
            }
            catch (Exception)
            {
                return new ApiResponse<string>("error", 400, "Xóa Phòng Chat Thất Bại!");
            }
        }
    }
}