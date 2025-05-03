using System.Text;
using System.Text.Json;
using AIBridges.Models;

namespace AIBridges.Services;

public class AzureVisionV1 : IAIService
{
    public ValueTask InitializeAsync()
    {
        // Initialization logic if needed
        return ValueTask.CompletedTask;
    }

    public async Task<object> ProcessRequestAsync(AIBridgeRequest request, HttpRequest requestBody)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", request.Key);

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{request.Endpoint}/vision/v1.0/models/{request.Model}/analyze?api-version=2023-05-15", content);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error calling Azure Vision: {response.ReasonPhrase}");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }
}