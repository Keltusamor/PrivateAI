namespace Keltusamor.PrivateAI;

public sealed record PrivateAiOptions
{
    public const string SectionName = "PrivateAi";

    public string ApiKey { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
}
