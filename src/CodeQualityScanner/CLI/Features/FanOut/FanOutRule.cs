namespace Amolenk.CodeQualityScanner.CLI.Features.FanOut;

public static class FanOutRule
{
    private const double InternalWeight = 1.6;
    private const double ExternalWeight = 0.4;
    
    public static bool Run(IFanOutProvider provider, double threshold)
    {
        var fanOutCounts = provider.GetFanOutCounts();
        
        var weightedFanOuts = CalculateWeightedFanOut(fanOutCounts);
        
        var average = weightedFanOuts.Values.Average();
        if (average > threshold)
        {
            Console.WriteLine($"Average fan out is {average}, which is higher than the threshold of {threshold}.");
            
            foreach (var (name, fanOut) in weightedFanOuts
                         .Where(x => x.Value > threshold)
                         .OrderByDescending(x => x.Value))
            {
                Console.WriteLine($"{name} has high fan out of {fanOut}.");
            }
            
            return false;
        }
        
        Console.WriteLine($"Average fan out is {average}, which is lower than the threshold of {threshold}.");
        return true;
    }

    private static Dictionary<string, double> CalculateWeightedFanOut(Dictionary<string, (int, int)> fanOut)
    {
        var result = new Dictionary<string, double>();

        foreach (var entry in fanOut)
        {
            var (@internal, @external) = entry.Value;
            result[entry.Key] = (@internal * InternalWeight) + (@external * ExternalWeight);
        }

        return result;
    }
}
