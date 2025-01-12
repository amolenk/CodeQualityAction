using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Amolenk.CodeQualityScanner.CLI.Features.FanOut;

public class MonoCecilFanOutProvider : IFanOutProvider
{
    private readonly string _binaryFolder;
    private readonly string _assemblyPrefix;
    private readonly bool _verbose;
    
    public MonoCecilFanOutProvider(string binaryFolder, string assemblyPrefix, bool verbose = false)
    {
        _binaryFolder = binaryFolder;
        _assemblyPrefix = assemblyPrefix;
        _verbose = verbose;
    }
    
    public Dictionary<string, (int, int)> GetFanOutCounts()
    {
        var result = new Dictionary<string, (int, int)>();
        
        foreach (var assemblyPath in Directory.GetFiles(_binaryFolder, $"{_assemblyPrefix}*.dll"))
        {
            GetFanOutCounts(assemblyPath, result);
        }

        return result;
    }

    public void GetFanOutCounts(string assemblyPath, Dictionary<string, (int, int)> result)
    {
        var module = ModuleDefinition.ReadModule(assemblyPath);
        foreach (var type in module.Types.Where(t => t.FullName.StartsWith(_assemblyPrefix)))
        {
            var fanOut = CalculateFanOut(type);

            result.Add(type.FullName, fanOut);
        }
    }

    private (int, int) CalculateFanOut(TypeDefinition typeDef)
    {
        var collector = CollectFanOutTypes(typeDef);

        var internalTypes = collector.GetInternalFanOutTypes(_assemblyPrefix).ToList();
        var externalTypes = collector.GetExternalFanOutTypes(_assemblyPrefix).ToList();

        Console.WriteLine($"{typeDef.FullName}: {internalTypes.Count} internal, {externalTypes.Count} external");

        if (_verbose)
        {
            foreach (var internalType in internalTypes)
            {
                Console.WriteLine($"{typeDef.FullName} -> {internalType} (Internal)");
            }

            foreach (var externalType in externalTypes)
            {
                Console.WriteLine($"{typeDef.FullName} -> {externalType} (External)");
            }
        }

        return (internalTypes.Count, externalTypes.Count);
    }
    
    private FanOutCollector CollectFanOutTypes(TypeDefinition typeDef)
    {
        var collector = new FanOutCollector();

        // Analyze methods
        foreach (var method in typeDef.Methods)
        {
            if (!method.HasBody) continue;

            foreach (var instruction in method.Body.Instructions)
            {
                AnalyzeInstruction(instruction, collector);
            }
        }

        // Analyze fields
        foreach (var field in typeDef.Fields)
        {
            collector.AddType(field.FieldType);
        }

        // Analyze properties
        foreach (var property in typeDef.Properties)
        {
            collector.AddType(property.PropertyType);
            
            // Analyze property getter method
            if (property.GetMethod != null && property.GetMethod.HasBody)
            {
                foreach (var instruction in property.GetMethod.Body.Instructions)
                {
                    AnalyzeInstruction(instruction, collector);
                }
            }
        }

        // Analyze events
        foreach (var eventDef in typeDef.Events)
        {
            collector.AddType(eventDef.EventType);
        }

        return collector;
    }
    
    private void AnalyzeInstruction(Instruction instruction, FanOutCollector collector)
    {
        if (instruction.Operand is TypeReference typeRef)
        {
            collector.AddType(typeRef);
        }
        else if (instruction.Operand is MethodReference methodRef)
        {
            if (methodRef.Name == ".ctor")
            {
                collector.AddType(methodRef.DeclaringType);
            }
            else
            {
                // Add the return type
                collector.AddType(methodRef.ReturnType);
            }

            // Add the generic arguments
            if (methodRef is GenericInstanceMethod genericMethod)
            {
                foreach (var argument in genericMethod.GenericArguments)
                {
                    collector.AddType(argument);
                }
            }

            // Add the parameter types
            foreach (var parameter in methodRef.Parameters)
            {
                collector.AddType(parameter.ParameterType);
            }
            
            // Analyze the method body if available
            if (methodRef is MethodDefinition { HasBody: true } methodDef)
            {
                foreach (var instr in methodDef.Body.Instructions)
                {
                    // Console.WriteLine(instr);
//                    AnalyzeInstruction(instr, collector);
                }
            }
        }
    }
    
    private class FanOutCollector
    {
        private static readonly HashSet<string> AcceptableTypes =
        [
            "System.Array",
            "System.Nullable`1",
            "System.Object",
            "System.RuntimeFieldHandle",
            "System.String",
            "System.Void"
        ];
        
        public List<string> CollectedTypes { get; } = new List<string>();

        public void AddType(TypeReference typeRef)
        {
            // Handle generic instance types
            if (typeRef is GenericInstanceType genericInstance)
            {
                if (genericInstance.ElementType != null)
                {
                    AddType(genericInstance.ElementType);
                }

                foreach (var argument in genericInstance.GenericArguments)
                {
                    AddType(argument);
                }

                return;
            }

            // Handle generic type parameters
            if (typeRef.IsGenericParameter)
            {
                var genericParam = (GenericParameter)typeRef;
                if (genericParam.DeclaringType != null)
                {
                    // Type parameter of a type
                    AddType(genericParam.DeclaringType);
                }
                else if (genericParam.DeclaringMethod != null)
                {
                    // Type parameter of a method
                    AddType(genericParam.DeclaringMethod.DeclaringType);
                }

                return;
            }
            
            // Ignore interfaces
            if (IsInterface(typeRef))
            {
                return;
            }
            
            if (IsAcceptable(typeRef))
            {
                return;
            }

            if (!CollectedTypes.Contains(typeRef.FullName))
            {
                CollectedTypes.Add(typeRef.FullName);
            }
        }

        public IEnumerable<string> GetInternalFanOutTypes(string namespacePrefix)
        {
            return CollectedTypes.Where(t => t.StartsWith(namespacePrefix));
        }

        public IEnumerable<string> GetExternalFanOutTypes(string namespacePrefix)
        {
            return CollectedTypes.Where(t => !t.StartsWith(namespacePrefix));
        }

        private bool IsAcceptable(TypeReference typeRef)
        {
            if (typeRef.IsPrimitive)
            {
                return true;
            }
            
            // Accept arrays of value types
            if (typeRef is ArrayType arrayType && arrayType.ElementType.IsValueType)
            {
                return true;
            }

            // Accept types in the acceptable list
            if (AcceptableTypes.Contains(typeRef.FullName))
            {
                return true;
            }
            
            return false;
        }
        
        private static bool IsInterface(TypeReference typeRef)
        {
            // Let's just assume good naming conventions.
            return typeRef.Name[0] == 'I' && char.IsUpper(typeRef.Name[1]);
        }
    }
}