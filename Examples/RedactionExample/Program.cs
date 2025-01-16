using Keltusamor.PrivateAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration((context, config) =>
{
    config.AddUserSecrets<Program>();
});

builder.ConfigureServices((context, services) =>
{
    services.AddPrivateAi(context.Configuration);
    services.AddSingleton<Examples>();
});

var app = builder.Build();

var examples = app.Services.GetRequiredService<Examples>();

await examples.RedactAsync();
//await examples.ReIdentifyAsync();
//await examples.EnableDisableEntitiesAsync();
//await examples.FilterTextAsync();
//await examples.GenerateSyntheticDataAsync();

class Examples(IPrivateAiClient PrivateAiClient)
{
    private readonly string _text = "My name is Dominik and I was born on 02.03.1995. I was a software developer at Miele. Now I work as a freelancer.";

    public async Task<ProcessTextResponse> RedactAsync()
    {
        var processTextRequest = new ProcessTextRequest
        {
            Text = [_text],
        };

        var result = (await PrivateAiClient.ProcessTextAsync(processTextRequest)).First();

        Console.WriteLine(result.ProcessedText);
        foreach (var (entity, index) in result.Entities.Select((x, i) => (x, i)))
        {
            Console.WriteLine($"Redaction marker {index}: {entity.ProcessedText} | Original text: {entity.Text}");
        }

        return result;
    }

    public async Task ReIdentifyAsync()
    {
        var redacted = await RedactAsync();
        var reIdentifyTextRequest = ReIdentifyTextRequest.CreateFrom(redacted);

        var result = (await PrivateAiClient.ReIdentifyTextAsync(reIdentifyTextRequest)).First();
        Console.WriteLine($"Re-identified text: {result}");
    }

    public async Task EnableDisableEntitiesAsync()
    {
        var processTextRequest = new ProcessTextRequest
        {
            Text = [_text],
            EntityDetection = new EntityDetection
            {
                EntityTypes =
                [
                    EntityTypeSelector.Enable(["NAME", "NAME_GIVEN", "DOB"]),
                    EntityTypeSelector.Disable(["OCCUPATION", "ORGANIZATION"]),
                ],
            },
        };
        var result = (await PrivateAiClient.ProcessTextAsync(processTextRequest)).First();
        Console.WriteLine(result.ProcessedText);
        foreach (var (entity, index) in result.Entities.Select((x, i) => (x, i)))
        {
            Console.WriteLine($"Redaction marker {index}: {entity.ProcessedText} | Original text: {entity.Text}");
        }
    }

    public async Task FilterTextAsync()
    {
        var processTextRequest = new ProcessTextRequest
        {
            Text = [_text],
            EntityDetection = new EntityDetection
            {
                Filter = [
                    Filter.Allow(@"\b(0[1-9]|[12][0-9]|3[01])\.(0[1-9]|1[0-2])\.(\d{4})\b"),
                    Filter.Block("PHYSICAL_ATTRIBUTE", @"\bMy name\b", 1),
                    Filter.AllowText(@"I was a.*?as a freelancer"),
                ],
            },
        };
        var result = (await PrivateAiClient.ProcessTextAsync(processTextRequest)).First();
        Console.WriteLine(result.ProcessedText);
        foreach (var (entity, index) in result.Entities.Select((x, i) => (x, i)))
        {
            Console.WriteLine($"Redaction marker {index}: {entity.ProcessedText} | Original text: {entity.Text}");
        }
    }

    public async Task GenerateSyntheticDataAsync()
    {
        var processTextRequest = new ProcessTextRequest
        {
            Text = [_text],
            ProcessedText = ProcessedText.Synthetic(SyntheticEntityAccuracy.StandardAutomatic, true, CoreferenceResolutionMode.Heuristics),
        };
        var result = (await PrivateAiClient.ProcessTextAsync(processTextRequest)).First();
        Console.WriteLine(result.ProcessedText);
        foreach (var (entity, index) in result.Entities.Select((x, i) => (x, i)))
        {
            Console.WriteLine($"Redaction marker {index}: {entity.ProcessedText} | Original text: {entity.Text}");
        }
    }
}
