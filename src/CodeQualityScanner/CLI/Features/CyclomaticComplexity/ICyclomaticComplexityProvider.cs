namespace Amolenk.CodeQualityScanner.CLI.Features.CyclomaticComplexity;

public interface ICyclomaticComplexityProvider
{
    Dictionary<string, double> GetCyclomaticComplexity();
}