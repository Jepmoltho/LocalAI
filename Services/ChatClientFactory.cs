using Anthropic;
using LocalAI.Configuration;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OllamaSharp;

namespace LocalAI.Services;

/// <summary>
/// Creates an IChatClient for a configured model. This is the only place that knows about specific providers.
/// </summary>
public class ChatClientFactory
{
    private const string DefaultOllamaEndpoint = "http://localhost:11434";
    private const string AnthropicApiKeyName = "ANTHROPIC_API_KEY";

    private readonly IConfiguration _configuration;

    public ChatClientFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IChatClient Create(ModelSettings model)
    {
        switch (model.Provider)
        {
            case ModelProvider.Ollama:
                return CreateOllamaClient(model);

            case ModelProvider.Anthropic:
                return CreateAnthropicClient(model);

            default:
                throw new NotSupportedException($"Provider '{model.Provider}' is not supported.");
        }
    }

    private IChatClient CreateOllamaClient(ModelSettings model)
    {
        string endpoint = DefaultOllamaEndpoint;
        if (!string.IsNullOrWhiteSpace(model.Endpoint))
        {
            endpoint = model.Endpoint;
        }

        return new OllamaApiClient(new Uri(endpoint), model.ModelId);
    }

    private IChatClient CreateAnthropicClient(ModelSettings model)
    {
        string? apiKey = _configuration[AnthropicApiKeyName];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                $"{AnthropicApiKeyName} is not set. Add it to your .env file as: {AnthropicApiKeyName}=sk-ant-...");
        }

        AnthropicClient anthropicClient = new AnthropicClient { ApiKey = apiKey };
        return anthropicClient.AsIChatClient(model.ModelId);
    }
}
