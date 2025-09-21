using System.Text;
using System.Text.Json;
using AIBridges.Models;

namespace AIBridges.Services;

public class AzureVisionV2 : IAIService
{
    public ValueTask InitializeAsync(CancellationToken cancellationToken)
    {
        // Initialization logic if needed
        return ValueTask.CompletedTask;
    }

    public async Task<object> ProcessRequestAsync(AIBridgeRequest request, HttpRequest requestBody, CancellationToken cancellationToken)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", request.Key);

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{request.Endpoint}/vision/v2.0/{request.Model}?api-version=2023-05-15", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error calling Azure Vision V2: {response.ReasonPhrase}");
        }

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        return responseBody;
    }
}