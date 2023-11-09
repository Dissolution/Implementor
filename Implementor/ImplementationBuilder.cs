using System.Diagnostics;

namespace Implementor;

public sealed record class InterfaceImplementation(
    string Namespace,
    string InterfaceName,
    string ClassName,
    string Code,
    Type[] CtorParamTypes);

public class ImplementationBuilder
{
    public TypeDeclarationSyntax TypeDeclaration { get; }
    public INamedTypeSymbol TypeSymbol { get; }
    public AttributeData ImplementAttributeData { get; }

    private string? _className;
    
    private readonly List<FieldSignature> _fields = new(0);
    private readonly List<PropertySignature> _properties = new(0);
    private readonly List<EventSignature> _events = new(0);
    private readonly List<MethodSignature> _constructors = new(0);
    private readonly List<MethodSignature> _methods = new(0);

    public ImplementationBuilder(
        TypeDeclarationSyntax typeDeclaration,
        INamedTypeSymbol typeSymbol,
        AttributeData implementAttributeData)
    {
        TypeDeclaration = typeDeclaration;
        TypeSymbol = typeSymbol;
        ImplementAttributeData = implementAttributeData;
    }

    private void ImplementProperty(IPropertySymbol propertySymbol)
    {
        var propertySignature = PropertySignature.Create(propertySymbol)!;
        // We're implementing this
        propertySignature.Keywords &= ~Keywords.Abstract;
        var fieldName = Naming.GetFieldName(propertySymbol);
        var fieldSignature = new FieldSignature
        {
            Name = fieldName,
            Visibility = Visibility.Instance | Visibility.Private,
            ValueType = propertySignature.ValueType,
        };
        if (propertySignature.Setter is null)
            fieldSignature.Keywords |= Keywords.Readonly;
        propertySignature.BackingField = fieldSignature;

        _fields.Add(fieldSignature);
        _properties.Add(propertySignature);
        //Debugger.Break();
    }


    private void WriteFields(CodeBuilder codeBuilder)
    {
        codeBuilder.Enumerate(_fields, (cb, field) => cb
            .IfAppend(field.Visibility, ' ')
            .IfAppend(field.Keywords, ' ')
            .IfAppend(field.ValueType, ' ')
            .Append(field.Name!)
            .Append(';')
            .NewLine());
    }

    private void WriteProperties(CodeBuilder codeBuilder)
    {
        codeBuilder.Delimit(static b => b.NewLine(), _properties, (cb, property) => cb
            .IfAppend(property.Visibility, ' ')
            .IfAppend(property.Keywords, ' ')
            .IfAppend(property.ValueType, ' ')
            .If(property.IsIndexer, ib => ib
                    .Append("this[")
                    .Delimit(", ", property.Parameters, static (b, p) => b
                        .Append(p.ValueType)
                        .Append(' ')
                        .Append(p.Name)
                        .If(p.DefaultValue.HasValue, d => d.Append(" = ").Append(p.DefaultValue.Value)))
                    .Append(']'),
                b =>
                {
                    Debug.Assert(property.Parameters.Count == 0);
                    b.Append(property.Name);
                })
            .If(property.BackingField is null,
                nfb => nfb
                    .Append(" {")
                    .If(property.Getter is not null, g => g
                        .If(property.Getter!.Visibility != property.Visibility,
                            b => b.Append(property.Getter.Visibility))
                        .Append(" get;"))
                    .If(property.Setter is not null, s => s
                        .If(property.Setter!.Visibility != property.Visibility,
                            b => b.Append(property.Setter.Visibility))
                        .Append(" set;"))
                    .Append(" }"),
                fb => fb
                    .If(property.Setter is null,
                        g => g.Append($" => {property.BackingField!.Name};"),
                        s => s.NewLine()
                            .Append($$"""
                                      {
                                          get => {{property.BackingField!.Name}};
                                          set => {{property.BackingField.Name}} = value;
                                      }
                                      """))));
    }

    private void WriteEvents(CodeBuilder codeBuilder)
    {
    }

    private void WriteConstructors(CodeBuilder codeBuilder)
    {
        // Look for required fields
        var requiredFields = _fields.Where(f => f.Keywords.HasFlag(Keywords.Readonly)).ToList();
        
        // Always at least this constructor
        codeBuilder.Append("public ").Append(_className).Append('(')
            .Delimit(", ", requiredFields, static (cb, field) =>
            {
                cb.Append(field.ValueType)
                    .Append(' ')
                    .Append(Naming.GetVariableName(field.Name));
            }).Append(')').NewLine()
            .BracketBlock(ib => ib
                .Delimit(static b => b.NewLine(), requiredFields, static (cb, field) =>
                {
                    cb.Append(field.Name)
                        .Append(" = ")
                        .Append(Naming.GetVariableName(field.Name))
                        .Append(';');
                }));

        // Then optional fields
        //var optionalFields = _fields.Where(f => !f.Keywords.HasFlag(Keywords.Readonly)).ToList();
        
        
    }

    private void WriteMethods(CodeBuilder codeBuilder)
    {
    }

    public InterfaceImplementation GetSourceFile()
    {
        // What interfaces do we implement?
        var interfaces = this.TypeSymbol.AllInterfaces;

        if (!interfaces.IsDefaultOrEmpty)
        {
            // We'll sub-process each
            Debugger.Break();
        }

        var properties = this.TypeSymbol.GetMembers().OfType<IPropertySymbol>().ToList();
        if (properties.Count > 0)
        {
            properties.ForEach(p => ImplementProperty(p));
        }

        string @namespace = TypeSymbol.GetFqNamespace();
        string interfaceName = TypeSymbol.Name;
        _className = Naming.GetImplementationName(interfaceName);


        using var fileBuilder = new CSharpFileBuilder();
        fileBuilder.AutoGeneratedHeader()
            .Nullable(true)
            .Namespace(@namespace)
            .Code
            .Append($$"""
                      public class {{_className}} : {{interfaceName}}
                      {
                          {{(CBA)WriteFields}}
                          {{(CBA)WriteProperties}}
                          {{(CBA)WriteEvents}}
                          {{(CBA)WriteConstructors}}
                          {{(CBA)WriteMethods}}
                      }
                      """);

        string code = fileBuilder.GetSourceCode();
        Console.Clear();
        Console.WriteLine(code);
        return new(@namespace, interfaceName, _className, code, Array.Empty<Type>());
    }
}