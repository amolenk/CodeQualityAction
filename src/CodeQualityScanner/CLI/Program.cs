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

if (op == "cc-cobertura")
{
    if (args.Length != 2)
    {
        Console.WriteLine("Usage: cc-cobertura <input-path> <threshold>");
        return 1;
    }
    
    var input = CoberturaInput.FromFile(args[0]);
    var threshold = double.Parse(args[1]);

    CyclomaticComplexityRule.Run(input, threshold);
}
else if (op == "cc-eslint")
{
    if (args.Length != 2)
    {
        Console.WriteLine("Usage: cc-eslint <input-path> <threshold>");
        return 1;
    }
    
    var input = ESLintInput.FromFile(args[0]);
    var threshold = double.Parse(args[1]);

    CyclomaticComplexityRule.Run(input, threshold);
}
else
{
    Console.WriteLine($"Unknown operation: {op}");
    return 1;
}

return 0;
