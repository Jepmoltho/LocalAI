using System.Diagnostics;
using LocalAI.Configuration;
using LocalAI.Results;
using Microsoft.Extensions.AI;

namespace LocalAI.Services;

/// <summary>
/// Sends a prompt to one model and measures latency, token usage and cost.
/// </summary>
public class ModelRunner
{
    private readonly ModelSettings _settings;
    private readonly IChatClient _client;
    private readonly ChatOptions _options;

    public ModelRunner(ModelSettings settings, IChatClient client)
    {
        _settings = settings;
        _client = client;
        _options = CreateOptions(settings);
    }

    public string DisplayName => _settings.DisplayName;

    public async Task<ModelResult> RunAsync(List<ChatMessage> messages, CancellationToken cancellationToken = default)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        ChatResponse response;
        try
        {
            response = await _client.GetResponseAsync(messages, _options, cancellationToken);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // One failing model shouldn't stop the others from answering.
            return new ModelResult
            {
                DisplayName = _settings.DisplayName,
                Succeeded = false,
                ErrorMessage = $"{ex.GetType().Name}: {ex.Message}",
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
            };
        }

        stopwatch.Stop();

        long inputTokens = 0;
        long outputTokens = 0;
        if (response.Usage != null)
        {
            inputTokens = response.Usage.InputTokenCount ?? 0;
            outputTokens = response.Usage.OutputTokenCount ?? 0;
        }

        return new ModelResult
        {
            DisplayName = _settings.DisplayName,
            Succeeded = true,
            Text = response.Text,
            ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
            InputTokens = inputTokens,
            OutputTokens = outputTokens,
            Cost = CostCalculator.Calculate(inputTokens, outputTokens, _settings)
        };
    }

    private static ChatOptions CreateOptions(ModelSettings settings)
    {
        ChatOptions options = new ChatOptions();
        options.MaxOutputTokens = settings.MaxOutputTokens;

        // Only send a temperature when one is configured. Some models reject it.
        if (settings.Temperature.HasValue)
        {
            options.Temperature = settings.Temperature.Value;
        }

        return options;
    }
}
