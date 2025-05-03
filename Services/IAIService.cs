using AIBridges.Models;

namespace AIBridges.Services
{
    public interface IAIService
    {
        ValueTask InitializeAsync();
        Task<string> ProcessRequestAsync(AIBridgeRequest request, object requestBody);
    }
}