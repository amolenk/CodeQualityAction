using System.Text.Json;
using System.Text.RegularExpressions;
using Amolenk.CodeQualityScanner.CLI.Features.CyclomaticComplexity;

namespace Amolenk.CodeQualityScanner.CLI.Features.ESLint;

public class ESLintInput : ICyclomaticComplexityProvider
{
    private readonly JsonDocument _document;
    
    private static readonly Regex ComplexityRegex = new(@"complexity of (\d+)\.", RegexOptions.Compiled);
    
    private ESLintInput(JsonDocument document)
    {
        _document = document;
    }
    
    public static ESLintInput FromFile(string path)
    {
        try
        {
            var jsonContent = File.ReadAllText(path);
            return new ESLintInput(JsonDocument.Parse(jsonContent));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing ESLint file: {ex.Message}");
            throw;
        }
    }

    public Dictionary<string, double> GetCyclomaticComplexity()
    {
        var result = new Dictionary<string, double>();
        
        foreach (var file in _document.RootElement.EnumerateArray())
        {
            var filePath = file.GetProperty("filePath").GetString();

            var messages = file.GetProperty("messages");
            foreach (var message in messages.EnumerateArray())
            {
                var ruleId = message.GetProperty("ruleId").GetString();
                if (ruleId != "complexity") continue;
                
                var complexityMessage = message.GetProperty("message").GetString()!;
                var line = message.GetProperty("line").GetInt32();
                var column = message.GetProperty("column").GetInt32();

                var key = $"{filePath}:{line}:{column}";

                var match = ComplexityRegex.Match(complexityMessage);
                var complexity = double.Parse(match.Groups[1].Value);
                    
                result.Add(key, complexity);
            }
        }

        return result;
    }
}