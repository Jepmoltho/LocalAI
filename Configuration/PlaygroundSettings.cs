namespace LocalAI.Configuration;

/// <summary>
/// Settings bound from the Playground section of appsettings.json.
/// </summary>
public class PlaygroundSettings
{
    public const string SectionName = "Playground";

    public string SystemPrompt { get; set; } = string.Empty;

    public List<ModelSettings> Models { get; set; } = new List<ModelSettings>();
}
