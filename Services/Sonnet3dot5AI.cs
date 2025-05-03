using System.Text;
using System.Text.Json;
using AIBridges.Models;

namespace AIBridges.Services;

public class Sonet3Dot5AI : IAIService
{
    public ValueTask InitializeAsync()
    {
        // Initialization logic if needed
        return ValueTask.CompletedTask;
    }

    public async Task<string> ProcessRequestAsync(AIBridgeRequest request, object requestBody)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {request.Key}");

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{request.Endpoint}/openai/deployments/{request.Model}/completions?api-version=2023-05-15", content);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error calling Sonet 3.5 AI: {response.ReasonPhrase}");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }
}