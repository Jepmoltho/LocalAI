namespace LocalAI.Embeddings;

/// <summary>
/// One cocktail and how similar it is to the search query.
/// </summary>
public class SearchResult
{
    public Cocktail Cocktail { get; init; } = new Cocktail();

    /// <summary>
    /// Cosine similarity between the query and the cocktail. Higher means closer in meaning.
    /// </summary>
    public double Score { get; init; }
}
