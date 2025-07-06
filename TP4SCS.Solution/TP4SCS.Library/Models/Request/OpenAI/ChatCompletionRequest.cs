using System.Text.Json.Serialization;

namespace TP4SCS.Library.Models.Request.OpenAI
{
    public class ChatCompletionRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; } = new List<Message>();

        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; }

    }

    public class Choice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; } = new Message();

        [JsonPropertyName("logprobs")]
        public object Logprobs { get; set; } = new object();

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; } = string.Empty;
    }

    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }

    public class ChatCompletionResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("object")]
        public string Object { get; set; } = string.Empty;

        [JsonPropertyName("created")]
        public int Created { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }
         = string.Empty;
        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; } = new List<Choice>();

        [JsonPropertyName("usage")]
        public Usage Usage { get; set; } = new Usage();

        [JsonPropertyName("system_fingerprint")]
        public object SystemFingerprint { get; set; } = new object();
    }

    public class Usage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }
}
