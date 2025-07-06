using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using TP4SCS.Library.Models.Request.OpenAI;
using TP4SCS.Library.Models.Response.Feedback;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class OpenAIService : IOpenAIService
    {
        private readonly IConfiguration _configuration;
        private const string OpenAIEndpoint = "https://api.openai.com/v1/chat/completions";
        private const string Model = "gpt-4o";

        public OpenAIService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> ValidateFeedbackContentAsync(HttpClient httpClient, string content)
        {
            string relativePath = _configuration["WordBlacklist"]!;
            string blacklistPath = Path.Combine(AppContext.BaseDirectory, relativePath);

            string blacklistJson = await File.ReadAllTextAsync(blacklistPath);

            var blacklist = JsonSerializer.Deserialize<List<WordBlacklistResponse>>(blacklistJson) ?? new List<WordBlacklistResponse>();

            var blacklistDetails = string.Join("\n",
                blacklist.GroupBy(item => item.Note)
                         .Select(group => $"{group.Key}: {string.Join(", ", group.Select(item => item.Word))}"));

            var prompt = $@"Danh sách các từ ngữ bị cấm: {blacklistDetails}" +
                $"Dùng tất cả kiến thức về ngôn ngữ của bạn và đặc biệt là tiếng Việt để phân tích nội dung đánh giá sau." +
                $"Trả về 'Valid' nếu nội dung đánh giá phù hợp, và 'Invalid' nếu nội dung giống từ cấm trong danh sách các từ bị cấm dưới mọi biến thể có thể ghi được:" +
                $"\n\n\"{content}\"";

            var completionRequest = new ChatCompletionRequest
            {
                Model = Model,
                MaxTokens = 1000,
                Messages = new List<Message>
                {
                    new Message
                    {
                        Role = "user",
                        Content = prompt
                    }
                }
            };

            using var httpReq = new HttpRequestMessage(HttpMethod.Post, OpenAIEndpoint);
            httpReq.Headers.Add("Authorization", $"Bearer {_configuration["OpenAIKey"]}");

            string requestString = JsonSerializer.Serialize(completionRequest);
            httpReq.Content = new StringContent(requestString, Encoding.UTF8, "application/json");

            using HttpResponseMessage httpResponse = await httpClient.SendAsync(httpReq);

            var responseContent = await httpResponse.Content.ReadAsStringAsync();

            var completionResponse = httpResponse.IsSuccessStatusCode ? JsonSerializer.Deserialize<ChatCompletionResponse>(await httpResponse.Content.ReadAsStringAsync()) : null;

            var analysisResult = completionResponse?.Choices?[0]?.Message?.Content;

            return string.Equals(analysisResult, "Valid", StringComparison.OrdinalIgnoreCase);
        }
    }
}
