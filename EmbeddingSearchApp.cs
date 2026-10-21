using System.Diagnostics;
using LocalAI.Configuration;
using LocalAI.Embeddings;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OllamaSharp;

namespace LocalAI;

/// <summary>
/// Semantic search experiment: embeds a small cocktail catalogue, then ranks the cocktails
/// by how close their meaning is to whatever you type.
/// </summary>
public class EmbeddingSearchApp
{
    // nomic-embed-text was trained with these prefixes. Using them noticeably improves search quality.
    // Other embedding models have other conventions (or none), so always check the model card.
    private const string DocumentPrefix = "search_document: ";
    private const string QueryPrefix = "search_query: ";

    private readonly EmbeddingSettings _settings;

    public EmbeddingSearchApp(IOptions<EmbeddingSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task RunAsync()
    {
        // OllamaApiClient implements IEmbeddingGenerator, just like it implements IChatClient.
        // The model name decides which one you're really talking to: an embedding model or a chat model.
        using OllamaApiClient ollamaClient = new OllamaApiClient(new Uri(_settings.Endpoint), _settings.ModelId);
        IEmbeddingGenerator<string, Embedding<float>> generator = ollamaClient;

        List<Cocktail> cocktails = CocktailCatalog.GetAll();
        List<float[]> cocktailVectors = await EmbedCocktailsAsync(generator, cocktails);

        Console.WriteLine("\nType what you're in the mood for. Press Enter on an empty line to exit.");

        while (true)
        {
            Console.Write("\nSearch: ");
            string? query = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(query))
            {
                break;
            }

            float[] queryVector = await EmbedTextAsync(generator, QueryPrefix + query);

            List<SearchResult> results = new List<SearchResult>();
            for (int i = 0; i < cocktails.Count; i++)
            {
                double score = VectorMath.CosineSimilarity(queryVector, cocktailVectors[i]);
                results.Add(new SearchResult { Cocktail = cocktails[i], Score = score });
            }

            // Highest score first
            results.Sort((first, second) => second.Score.CompareTo(first.Score));

            PrintResults(results);
        }
    }

    private async Task<List<float[]>> EmbedCocktailsAsync(IEmbeddingGenerator<string, Embedding<float>> generator, List<Cocktail> cocktails)
    {
        // What we embed is a choice: here name + description. Later you might add ingredients or flavour notes.
        List<string> documents = new List<string>();
        foreach (Cocktail cocktail in cocktails)
        {
            documents.Add(DocumentPrefix + cocktail.Name + ": " + cocktail.Description);
        }

        Console.WriteLine($"Embedding {documents.Count} cocktails with {_settings.ModelId}...");

        Stopwatch stopwatch = Stopwatch.StartNew();
        GeneratedEmbeddings<Embedding<float>> embeddings = await generator.GenerateAsync(documents);
        stopwatch.Stop();

        List<float[]> vectors = new List<float[]>();
        foreach (Embedding<float> embedding in embeddings)
        {
            vectors.Add(embedding.Vector.ToArray());
        }

        // Show what an embedding actually is: a long list of numbers.
        float[] first = vectors[0];
        Console.WriteLine($"Done in {stopwatch.ElapsedMilliseconds} ms. Each embedding has {first.Length} dimensions.");
        Console.WriteLine($"First 5 numbers of '{cocktails[0].Name}': " +
            $"{first[0]:F4}, {first[1]:F4}, {first[2]:F4}, {first[3]:F4}, {first[4]:F4} ...");

        return vectors;
    }

    private static async Task<float[]> EmbedTextAsync(IEmbeddingGenerator<string, Embedding<float>> generator, string text)
    {
        List<string> input = new List<string>();
        input.Add(text);

        GeneratedEmbeddings<Embedding<float>> embeddings = await generator.GenerateAsync(input);
        return embeddings[0].Vector.ToArray();
    }

    private static void PrintResults(List<SearchResult> results)
    {
        // Print every cocktail, not just the top few, so you can see how close or far apart the scores are.
        for (int i = 0; i < results.Count; i++)
        {
            SearchResult result = results[i];
            Console.WriteLine($"  {i + 1,2}. {result.Score:F3}  {result.Cocktail.Name}");
        }
    }
}
