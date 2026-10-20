namespace LocalAI.Results;

/// <summary>
/// The outcome of sending one prompt to one model: the answer, or the error, plus the measurements.
/// </summary>
public class ModelResult
{
    public string DisplayName { get; init; } = string.Empty;

    public bool Succeeded { get; init; }

    public string Text { get; init; } = string.Empty;

    public string? ErrorMessage { get; init; }

    public long ElapsedMilliseconds { get; init; }

    public long InputTokens { get; init; }

    public long OutputTokens { get; init; }

    public double Cost { get; init; }
}
