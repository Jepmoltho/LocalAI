using LocalAI;
using LocalAI.Configuration;
using LocalAI.Output;
using LocalAI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Load .env into environment variables. This must run BEFORE the host is created,
// because the host reads environment variables into IConfiguration when it is built.
// TraversePath() searches upwards from bin\Debug\net10.0 until it finds the .env file.
DotNetEnv.Env.TraversePath().Load();

// The host loads appsettings.json and environment variables automatically.
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<PlaygroundSettings>(builder.Configuration.GetSection(PlaygroundSettings.SectionName));
builder.Services.Configure<EmbeddingSettings>(builder.Configuration.GetSection(EmbeddingSettings.SectionName));
builder.Services.AddSingleton<ChatClientFactory>();
builder.Services.AddSingleton<ConsoleReporter>();
builder.Services.AddSingleton<PlaygroundApp>();
builder.Services.AddSingleton<EmbeddingSearchApp>();

using IHost host = builder.Build();

Console.WriteLine("Choose an experiment:");
Console.WriteLine("  1. Model playground (compare chat models)");
Console.WriteLine("  2. Embedding search (semantic cocktail search)");
Console.Write("> ");
string? choice = Console.ReadLine();

try
{
    if (choice == "2")
    {
        EmbeddingSearchApp embeddingApp = host.Services.GetRequiredService<EmbeddingSearchApp>();
        await embeddingApp.RunAsync();
    }
    else
    {
        PlaygroundApp playgroundApp = host.Services.GetRequiredService<PlaygroundApp>();
        await playgroundApp.RunAsync();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"\nThe application stopped because of an error: {ex.Message}");
    Console.WriteLine("Press Enter to exit.");
    Console.ReadLine();
}
