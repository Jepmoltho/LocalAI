using System.Diagnostics;
using Anthropic;
using Microsoft.Extensions.AI;
using OllamaSharp;
using DotNetEnv;

DotNetEnv.Env.TraversePath().Load();

// Local model
IChatClient localClient = new OllamaApiClient(new Uri("http://localhost:11434"), "llama3.2");

// Hosted model
string? apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("ANTHROPIC_API_KEY is not set. Press Enter to exit.");
    Console.ReadLine();
    return;
}
AnthropicClient anthropic = new AnthropicClient { ApiKey = apiKey };
IChatClient hostedClient = anthropic.AsIChatClient("claude-haiku-5-5");

// Fill these in from the Anthropic pricing page (USD per million tokens)
const double HostedInputPricePerMillion = 0.0;
const double HostedOutputPricePerMillion = 0.0;

// Local model options: Ollama supports temperature
ChatOptions localOptions = new ChatOptions();
localOptions.Temperature = 0.7f;
localOptions.MaxOutputTokens = 1024;

// Hosted model options: no temperature (rejected by this model)
ChatOptions hostedOptions = new ChatOptions();
hostedOptions.MaxOutputTokens = 1024; // Required by the Anthropic API

while (true)
{
    Console.Write("\nYou: ");
    string? input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input))
    {
        break;
    }

    List<ChatMessage> messages = new List<ChatMessage>();
    messages.Add(new ChatMessage(ChatRole.System, "You are a knowledgeable bartender. Keep answers short."));
    messages.Add(new ChatMessage(ChatRole.User, input));

    await AskModel("LOCAL  (llama3.2)", localClient, messages, localOptions, 0.0, 0.0);
    await AskModel("HOSTED (Claude Haiku)", hostedClient, messages, hostedOptions,
        HostedInputPricePerMillion, HostedOutputPricePerMillion);
}

static async Task AskModel(string label, IChatClient client, List<ChatMessage> messages,
    ChatOptions options, double inputPricePerMillion, double outputPricePerMillion)
{
    Stopwatch stopwatch = Stopwatch.StartNew();

    ChatResponse response;
    try
    {
        response = await client.GetResponseAsync(messages, options);
    }
    catch (Exception ex)
    {
        stopwatch.Stop();
        Console.WriteLine($"\n=== {label} FAILED after {stopwatch.ElapsedMilliseconds} ms ===");
        Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
        return;
    }

    stopwatch.Stop();

    Console.WriteLine($"\n=== {label} ===");
    Console.WriteLine(response.Text);

    long inputTokens = 0;
    long outputTokens = 0;
    if (response.Usage != null)
    {
        inputTokens = response.Usage.InputTokenCount ?? 0;
        outputTokens = response.Usage.OutputTokenCount ?? 0;
    }

    double cost = (inputTokens * inputPricePerMillion + outputTokens * outputPricePerMillion) / 1_000_000;

    Console.WriteLine($"[{stopwatch.ElapsedMilliseconds} ms | in: {inputTokens} | out: {outputTokens} | cost: ${cost:F6}]");
}