using LocalAI.Results;
using LocalAI.Services;

namespace LocalAI.Output;

/// <summary>
/// All console input and output lives here, so the rest of the app doesn't depend on the console.
/// </summary>
public class ConsoleReporter
{
    public void PrintWelcome(List<ModelRunner> runners)
    {
        Console.WriteLine("Model playground. Each prompt is sent to:");
        foreach (ModelRunner runner in runners)
        {
            Console.WriteLine($"  - {runner.DisplayName}");
        }
        Console.WriteLine("Press Enter on an empty line to exit.");
    }

    public string? ReadPrompt()
    {
        Console.Write("\nYou: ");
        return Console.ReadLine();
    }

    public void PrintResult(ModelResult result)
    {
        if (!result.Succeeded)
        {
            Console.WriteLine($"\n=== {result.DisplayName} FAILED after {result.ElapsedMilliseconds} ms ===");
            Console.WriteLine(result.ErrorMessage);
            return;
        }

        Console.WriteLine($"\n=== {result.DisplayName} ===");
        Console.WriteLine(result.Text);
        Console.WriteLine(
            $"[{result.ElapsedMilliseconds} ms | in: {result.InputTokens} | out: {result.OutputTokens} | cost: ${result.Cost:F6}]");
    }
}
