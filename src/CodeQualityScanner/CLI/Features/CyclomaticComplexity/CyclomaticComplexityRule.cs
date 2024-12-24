namespace Amolenk.CodeQualityScanner.CLI.Features.CyclomaticComplexity;

public static class CyclomaticComplexityRule
{
    public static bool Run(ICyclomaticComplexityProvider provider, double threshold)
    {
        var complexities = provider.GetCyclomaticComplexity();

        var average = complexities.Values.Average();
        if (average > threshold)
        {
            Console.WriteLine($"Average cyclomatic complexity is {average}, which is higher than the threshold of {threshold}.");
            
            foreach (var (name, complexity) in complexities
                         .Where(x => x.Value > threshold)
                         .OrderByDescending(x => x.Value))
            {
                Console.WriteLine($"{name} has high cyclomatic complexity of {complexity}.");
            }
            
            return false;
        }
        
        Console.WriteLine($"Average cyclomatic complexity is {average}, which is lower than the threshold of {threshold}.");
        return true;
    }
}
