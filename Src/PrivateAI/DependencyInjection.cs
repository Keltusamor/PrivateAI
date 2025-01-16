using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Keltusamor.PrivateAI;

public static class DependencyInjection
{
    public static IServiceCollection AddPrivateAi(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<PrivateAiOptions>(config.GetSection(PrivateAiOptions.SectionName));

        services.AddSingleton<IPrivateAiClient, PrivateAiClient>();
        services.AddHttpClient(PrivateAiOptions.SectionName, (services, client) =>
        {
            var options = services.GetRequiredService<IOptions<PrivateAiOptions>>().Value;
            client.BaseAddress = new Uri(options.Url);
            client.DefaultRequestHeaders.Add("x-api-key", options.ApiKey);
        });
        // .AddHttpMessageHandler(() => new LoggingHandler());

        return services;
    }
}
