namespace LocalAI.Configuration;

/// <summary>
/// Settings for one model, bound from an entry in Playground:Models in appsettings.json.
/// </summary>
public class ModelSettings
{
    public string DisplayName { get; set; } = string.Empty;

    public ModelProvider Provider { get; set; }

    public string ModelId { get; set; } = string.Empty;

    /// <summary>
    /// Only used by Ollama. Defaults to http://localhost:11434 when empty.
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// Leave empty for models that reject a temperature setting, such as newer Claude models.
    /// </summary>
    public float? Temperature { get; set; }

    /// <summary>
    /// Required by the Anthropic API (max_tokens).
    /// </summary>
    public int MaxOutputTokens { get; set; } = 1024;

    /// <summary>
    /// USD per million input tokens. Use 0 for local models.
    /// </summary>
    public double InputPricePerMillion { get; set; }

    /// <summary>
    /// USD per million output tokens. Use 0 for local models.
    /// </summary>
    public double OutputPricePerMillion { get; set; }
}
