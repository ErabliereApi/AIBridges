using AIBridges.Models;

namespace AIBridges.Services
{
    public interface IAIService
    {
        ValueTask InitializeAsync();
        Task<object> ProcessRequestAsync(AIBridgeRequest request, HttpRequest requestBody);
    }
}