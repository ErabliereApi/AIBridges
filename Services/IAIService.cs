using AIBridges.Models;

namespace AIBridges.Services
{
    public interface IAIService
    {
        ValueTask InitializeAsync(CancellationToken cancellationToken);
        Task<object> ProcessRequestAsync(AIBridgeRequest request, HttpRequest requestBody, CancellationToken cancellationToken);
    }
}