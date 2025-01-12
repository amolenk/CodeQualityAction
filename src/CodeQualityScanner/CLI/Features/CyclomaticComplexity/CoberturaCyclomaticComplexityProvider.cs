using System.Xml.Linq;

namespace Amolenk.CodeQualityScanner.CLI.Features.CyclomaticComplexity;

public class CoberturaCyclomaticComplexityProvider : ICyclomaticComplexityProvider
{
    private readonly XDocument _document;
    
    private CoberturaCyclomaticComplexityProvider(XDocument document)
    {
        _document = document;
    }
    
    public static CoberturaCyclomaticComplexityProvider FromFile(string path)
    {
        try
        {
            return new CoberturaCyclomaticComplexityProvider(XDocument.Load(path));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing Cobertura file: {ex.Message}");
            throw;
        }
    }

    public Dictionary<string, double> GetCyclomaticComplexity()
    {
        var methods = _document.Descendants("method");

        return methods
            .Select(m => new
            {
                Name = GetMethodFullName(m),
                Complexity = double.TryParse(m.Attribute("complexity")?.Value, out var value) ? value : (double?)null
            })
            .Where(v => v.Complexity.HasValue)
            .ToDictionary(x => x.Name, x => x.Complexity!.Value);
    }

    private string GetMethodFullName(XElement method)
    {
        var package = GetPackage(method);
        var ns = GetNamespace(method);
        var localName = method.Attribute("name")?.Value;
        var signature = method.Attribute("signature")?.Value;

        return $"{package}.{ns}.{localName}{signature}";
    }

    private static string GetNamespace(XElement element)
    {
        while (true)
        {
            if (element.Name.LocalName == "class")
            {
                return element.Attribute("name")?.Value ?? string.Empty;
            }

            if (element.Parent is null) return string.Empty;
            
            element = element.Parent;
        }
    }
    
    private static string GetPackage(XElement element)
    {
        while (true)
        {
            if (element.Name.LocalName == "package")
            {
                return element.Attribute("name")?.Value ?? string.Empty;
            }

            if (element.Parent is null) return string.Empty;
            
            element = element.Parent;
        }
    }

}