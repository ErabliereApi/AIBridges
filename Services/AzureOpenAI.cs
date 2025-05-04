using System.ComponentModel;
using System.Text;
using System.Text.Json;
using AIBridges.Attributes;
using AIBridges.Models;

namespace AIBridges.Services;

[Version("2.0")]
[Description("Azure OpenAI Service")]
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

        var bodyString = await requestBody.ReadFromJsonAsync<object>();

        var content = new StringContent(JsonSerializer.Serialize(bodyString), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{request.Endpoint}/openai/deployments/{request.Model}/completions?api-version=2023-05-15", content);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error calling Azure OpenAI: {response.ReasonPhrase}");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }
}