using Amolenk.CodeQualityScanner.CLI.Features.Cobertura;
using Amolenk.CodeQualityScanner.CLI.Features.CyclomaticComplexity;
using Amolenk.CodeQualityScanner.CLI.Features.ESLint;

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
        var input = CoberturaInput.FromFile(args[0]);
        var threshold = double.Parse(args[1]);

        succes = CyclomaticComplexityRule.Run(input, threshold);
        break;
    }
    case "cc-eslint" when args.Length != 2:
        Console.WriteLine("Usage: cc-eslint <input-path> <threshold>");
        break;
    case "cc-eslint":
    {
        var input = ESLintInput.FromFile(args[0]);
        var threshold = double.Parse(args[1]);

        succes = CyclomaticComplexityRule.Run(input, threshold);
        break;
    }
    default:
        Console.WriteLine($"Unknown operation: {op}");
        break;
}

return succes ? 0 : 1;
