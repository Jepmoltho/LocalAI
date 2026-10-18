using System.Diagnostics;
using Microsoft.Extensions.AI;
using OllamaSharp;

IChatClient client = new OllamaApiClient(new Uri("http://localhost:11434"), "llama3.2");

ChatOptions options = new ChatOptions();

options.Temperature = 0.7f;

List<ChatMessage> history = new List<ChatMessage>();
history.Add(new ChatMessage(ChatRole.System, "You are a knowledgeable bartender. Keep answers short."));

while (true) 
{
    Console.Write("\nYou: ");
    string? input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input))
    {
        break;
    }

    history.Add(new ChatMessage(ChatRole.User, input));

    Stopwatch stopwatch = Stopwatch.StartNew();
    ChatResponse response = await client.GetResponseAsync(history, options);
    Console.WriteLine($"(model: {response.ModelId})");
    stopwatch.Stop();

    Console.WriteLine($"\nAI: {response.Text}");
    Console.WriteLine($"[{stopwatch.ElapsedMilliseconds} ms");

    if (response.Usage != null)
    {
        Console.WriteLine($" | in: {response.Usage.InputTokenCount} tokens, out: {response.Usage.OutputTokenCount} tokens]");
    }

    history.AddRange(response.Messages);
}

