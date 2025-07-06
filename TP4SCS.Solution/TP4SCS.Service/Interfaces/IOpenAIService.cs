namespace TP4SCS.Services.Interfaces
{
    public interface IOpenAIService
    {
        Task<bool> ValidateFeedbackContentAsync(HttpClient httpClient, string content);
    }
}
