using LocalAI.Configuration;

namespace LocalAI.Services;

/// <summary>
/// Calculates the USD cost of a request from its token counts and the model's prices.
/// </summary>
public static class CostCalculator
{
    private const double TokensPerMillion = 1_000_000;

    public static double Calculate(long inputTokens, long outputTokens, ModelSettings model)
    {
        double inputCost = inputTokens * model.InputPricePerMillion / TokensPerMillion;
        double outputCost = outputTokens * model.OutputPricePerMillion / TokensPerMillion;
        return inputCost + outputCost;
    }
}
