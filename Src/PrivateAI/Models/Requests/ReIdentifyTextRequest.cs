namespace Keltusamor.PrivateAI;

public class ReIdentifyTextRequest
{
    public string[] ProcessedText { get; set; } = null!;
    public ReIdentifyTextEntity[] Entities { get; set; } = null!;
    public string? Model { get; set; }
    public bool? ReidentifySensitiveFields { get; set; }
    public string? ProjectId { get; set; }

    public static ReIdentifyTextRequest CreateFrom(ProcessTextResponse response)
    {
        return new ReIdentifyTextRequest
        {
            ProcessedText = [response.ProcessedText],
            Entities = ReIdentifyTextEntity.CreateFrom(response.Entities),
        };
    }
}

public class ReIdentifyTextEntity
{
    public string ProcessedText { get; set; } = null!;
    public string Text { get; set; } = null!;

    public static ReIdentifyTextEntity[] CreateFrom(ProcessTextEntity[] entities)
    {
        return entities.Select(entity => new ReIdentifyTextEntity
        {
            ProcessedText = entity.ProcessedText,
            Text = entity.Text
        }).ToArray();
    }
}
