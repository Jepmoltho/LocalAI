using LocalAI.Configuration;
using LocalAI.Output;
using LocalAI.Results;
using LocalAI.Services;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace LocalAI;

/// <summary>
/// The main loop: reads a prompt, sends it to every configured model and prints the results.
/// </summary>
public class PlaygroundApp
{
    private readonly PlaygroundSettings _settings;
    private readonly ChatClientFactory _clientFactory;
    private readonly ConsoleReporter _reporter;

    public PlaygroundApp(IOptions<PlaygroundSettings> settings, ChatClientFactory clientFactory, ConsoleReporter reporter)
    {
        _settings = settings.Value;
        _clientFactory = clientFactory;
        _reporter = reporter;
    }

    public async Task RunAsync()
    {
        List<ModelRunner> runners = CreateRunners();

        _reporter.PrintWelcome(runners);

        while (true)
        {
            string? input = _reporter.ReadPrompt();
            if (string.IsNullOrWhiteSpace(input))
            {
                break;
            }

            List<ChatMessage> messages = BuildMessages(input);

            // Models run one after another, not in parallel, so latency measurements don't affect each other.
            foreach (ModelRunner runner in runners)
            {
                ModelResult result = await runner.RunAsync(messages);
                _reporter.PrintResult(result);
            }
        }
    }

    private List<ModelRunner> CreateRunners()
    {
        if (_settings.Models.Count == 0)
        {
            throw new InvalidOperationException("No models are configured in appsettings.json under Playground:Models.");
        }

        List<ModelRunner> runners = new List<ModelRunner>();

        foreach (ModelSettings modelSettings in _settings.Models)
        {
            IChatClient client = _clientFactory.Create(modelSettings);
            ModelRunner runner = new ModelRunner(modelSettings, client);
            runners.Add(runner);
        }

        return runners;
    }

    private List<ChatMessage> BuildMessages(string input)
    {
        // A fresh conversation for every prompt, so each model gets an identical request.
        List<ChatMessage> messages = new List<ChatMessage>();
        messages.Add(new ChatMessage(ChatRole.System, _settings.SystemPrompt));
        messages.Add(new ChatMessage(ChatRole.User, input));
        return messages;
    }
}
