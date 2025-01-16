using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Keltusamor.PrivateAI;

public class PrivateAiClient(IHttpClientFactory HttpClientFactory) : IPrivateAiClient
{
    private readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters =
        {
            new JsonStringEnumConverter(new SnakeCaseNamingPolicy()),
        },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    public async Task<IList<ProcessTextResponse>> ProcessTextAsync(ProcessTextRequest request)
    {
        using HttpClient httpClient = HttpClientFactory.CreateClient(PrivateAiOptions.SectionName);

        var jsonContent = JsonSerializer.Serialize(request, JsonSerializerOptions);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await httpClient.PostAsync("process/text", content);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IList<ProcessTextResponse>>(responseBody, JsonSerializerOptions)!;
    }

    public async Task<IList<string>> ReIdentifyTextAsync(ReIdentifyTextRequest request)
    {
        using HttpClient httpClient = HttpClientFactory.CreateClient(PrivateAiOptions.SectionName);
        var jsonContent = JsonSerializer.Serialize(request, JsonSerializerOptions);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await httpClient.PostAsync("process/text/reidentify", content);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<IList<string>>(responseBody, JsonSerializerOptions)!;
    }
}
