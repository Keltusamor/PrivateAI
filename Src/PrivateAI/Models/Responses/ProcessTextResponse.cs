namespace Keltusamor.PrivateAI;

public class ProcessTextResponse
{
    public ProcessTextEntity[] Entities { get; set; } = null!;
    public bool EntitiesPresent { get; set; }
    public int CharactersProcessed { get; set; }
    public Dictionary<string, double> LanguagesDetected { get; set; } = null!;
    public string ProcessedText { get; set; } = null!;
}

public class ProcessTextEntity
{
    public string ProcessedText { get; set; } = null!;
    public string? Text { get; set; }
    public Location? Location { get; set; }
    public string BestLabel { get; set; } = null!;
    public Dictionary<string, double> Labels { get; set; } = null!;
    public ValidationResult[]? ValidationResults { get; set; }
}

public class ValidationResult
{
    public string Formatted { get; set; } = null!;
    public string ValidationMethod { get; set; } = null!;
    public string Status { get; set; } = null!;
}

public class Location
{
    public int SttIdx { get; set; }
    public int EndIdx { get; set; }
    public int SttIdxProcessed { get; set; }
    public int EndIdxProcessed { get; set; }
}
