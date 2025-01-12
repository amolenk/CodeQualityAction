namespace Amolenk.CodeQualityScanner.CLI.Features.FanOut;

public interface IFanOutProvider
{
    Dictionary<string, (int, int)> GetFanOutCounts();
}