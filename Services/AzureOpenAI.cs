using System.Text;
using System.Text.Json;
using AIBridges.Models;
using AIBridges.Services;

namespace AIBridges.Services;

public class AzureOpenAI : IAIService
{
    public ValueTask InitializeAsync()
    {
        // Initialization logic if needed
        return ValueTask.CompletedTask;
    }

    public async Task<object> ProcessRequestAsync(AIBridgeRequest request, HttpRequest requestBody)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {request.Key}");

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{request.Endpoint}/openai/deployments/{request.Model}/completions?api-version=2023-05-15", content);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error calling Azure OpenAI: {response.ReasonPhrase}");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }
}