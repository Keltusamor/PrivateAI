namespace Keltusamor.PrivateAI;

public class LoggingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Request: {request.Method} {request.RequestUri}");

        foreach (var header in request.Headers)
        {
            Console.WriteLine($"Request Header: {header.Key}: {string.Join(", ", header.Value)}");
        }

        if (request.Content != null)
        {
            var requestBody = await request.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine($"Request Body: {requestBody}");
        }

        var response = await base.SendAsync(request, cancellationToken);

        Console.WriteLine($"Response: {response.StatusCode}");

        return response;
    }
}
