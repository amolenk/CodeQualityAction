using Amolenk.CodeQualityScanner.CLI.Features.CyclomaticComplexity;
using Amolenk.CodeQualityScanner.CLI.Features.FanOut;

if (args.Length == 0)
{
    Console.WriteLine("No arguments provided.");
    return 1;
}

var op = args[0];
args = args.Skip(1).ToArray();

var succes = false;

switch (op)
{
    case "cc-cobertura" when args.Length != 2:
        Console.WriteLine("Usage: cc-cobertura <input-path> <threshold>");
        break;
    case "cc-cobertura":
    {
        var input = CoberturaCyclomaticComplexityProvider.FromFile(args[0]);
        var threshold = double.Parse(args[1]);

        succes = CyclomaticComplexityRule.Run(input, threshold);
        break;
    }
    case "cc-eslint" when args.Length != 2:
        Console.WriteLine("Usage: cc-eslint <input-path> <threshold>");
        break;
    case "cc-eslint":
    {
        var input = ESLintCyclomaticComplexityProvider.FromFile(args[0]);
        var threshold = double.Parse(args[1]);

        succes = CyclomaticComplexityRule.Run(input, threshold);
        break;
    }
    case "fo-dotnet" when args.Length != 3:
        Console.WriteLine("Usage: fo-dotnet <binary-folder> <assembly-filter> <threshold>");
        break;
    case "fo-dotnet":
    {
        var input = new MonoCecilFanOutProvider(args[0], args[1]);
        var threshold = double.Parse(args[2]);

        succes = FanOutRule.Run(input, threshold);
        break;
    }
    default:
        Console.WriteLine($"Unknown operation: {op}");
        break;
}

return succes ? 0 : 1;
