namespace Keltusamor.PrivateAI;

public class ProcessTextRequest
{
    public string[] Text { get; set; } = [];
    public bool? LinkBatch { get; set; }
    public EntityDetection? EntityDetection { get; set; }
    public string? ProjectId { get; set; }
    public ProcessedText? ProcessedText { get; set; }
}

public class EntityDetection
{
    public AccuracyMode? Accuracy { get; set; }
    public EntityTypeSelector[]? EntityTypes { get; set; }
    public Filter[]? Filter { get; set; }
    public bool? ReturnEntity { get; set; }
    public bool? EnableNonMaxSuppression { get; set; }
    public ValidationOption[]? ValidationOptions { get; set; }
}

public enum AccuracyMode
{
    Standard,
    StandardHigh,
    StandardHighMultilingual,
    StandardHighAutomatic,
    High,
    HighMultilingual,
    HighAutomatic
}

public class EntityTypeSelector
{
    public string? Type { get; set; }
    public string[]? Value { get; set; }

    public static EntityTypeSelector Enable(string[] value)
        => new()
        {
            Type = "ENABLE",
            Value = value
        };

    public static EntityTypeSelector Disable(string[] value)
    => new()
    {
        Type = "DISABLE",
        Value = value
    };
}

public class Filter
{
    public string? Type { get; set; }
    public string? Pattern { get; set; }
    public string? EntityType { get; set; }
    public double? Threshold { get; set; }

    public static Filter Allow(string pattern)
        => new() { Type = "ALLOW", Pattern = pattern };

    public static Filter Block(string entityType, string pattern, double threshold = 1.0)
        => new() { Type = "BLOCK", Pattern = pattern, EntityType = entityType, Threshold = threshold };

    public static Filter AllowText(string pattern)
        => new() { Type = "ALLOW_TEXT", Pattern = pattern };
}

public class ValidationOption
{
    public string Algorithm { get; set; } = "luhn";
    public string[]? OnEntities { get; set; }
}

public class ProcessedText
{
    public string? Type { get; set; }
    public string? Pattern { get; set; }
    public MarkerLanguage? MarkerLanguage { get; set; }
    public CoreferenceResolutionMode? CoreferenceResolution { get; set; }
    public string[]? MaskCharacter { get; set; }
    public SyntheticEntityAccuracy? SyntheticEntityAccuracy { get; set; }
    public bool? PreserveRelationships { get; set; }

    public static ProcessedText Marker(string? pattern = null, MarkerLanguage? markerLanguage = null, CoreferenceResolutionMode? coreferenceResolutionMode = null)
        => new()
        {
            Type = "MARKER",
            Pattern = pattern,
            MarkerLanguage = markerLanguage,
            CoreferenceResolution = coreferenceResolutionMode
        };

    public static ProcessedText Mask(string[]? maskCharacter = null)
        => new()
        {
            Type = "MASK",
            MaskCharacter = maskCharacter
        };

    public static ProcessedText Synthetic(SyntheticEntityAccuracy? syntheticEntityAccuracy = null, bool? preserveRelationships = null, CoreferenceResolutionMode? coreferenceResolution = null)
        => new()
        {
            Type = "SYNTHETIC",
            SyntheticEntityAccuracy = syntheticEntityAccuracy,
            PreserveRelationships = preserveRelationships,
            CoreferenceResolution = coreferenceResolution
        };

    public static class PatternType
    {
        public const string BestEntityType = "[BEST_ENTITY_TYPE]";
        public const string AllEntityTypes = "[ALL_ENTITY_TYPES]";
        public const string UniqueNumberedEntityType = "[UNIQUE_NUMBERED_ENTITY_TYPE]";
        public const string UniqueHashedEntityType = "[UNIQUE_HASHED_ENTITY_TYPE]";
    }
}

public enum MarkerLanguage
{
    Auto,
    En,
    Fr,
    De,
    Es,
    Hi,
    It,
    Ja,
    Ko,
    Nl,
    Pt,
    Ru,
    Tl,
    Uk,
    Zh
}

public enum CoreferenceResolutionMode
{
    Heuristics,
    Combined,
    ModelPrediction
}

public enum SyntheticEntityAccuracy
{
    Standard,
    StandardMultilingual,
    StandardAutomatic
}
