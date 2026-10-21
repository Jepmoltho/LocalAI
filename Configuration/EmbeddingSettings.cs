namespace LocalAI.Configuration;

/// <summary>
/// Settings bound from the Embeddings section of appsettings.json.
/// </summary>
public class EmbeddingSettings
{
    public const string SectionName = "Embeddings";

    public string ModelId { get; set; } = "nomic-embed-text";

    public string Endpoint { get; set; } = "http://localhost:11434";
}
