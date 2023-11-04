using Implementor.Text;

namespace Implementor.Scratch;

public sealed class Attributes : List<AttributeSignature>
{
    public static Attributes From(ISymbol? symbol)
    {
        var attributes = new Attributes();
        if (symbol is not null)
        {
            attributes.AddRange(
                symbol.GetAttributes()
                    .Select(static a => AttributeSignature.Create(a))
                    .Where(static a => a is not null)!);
        }

        return attributes;
    }
    
    public Attributes() : base(0){ }
    
    //
    // public Attributes(ISymbol symbol)
    // {
    //     _attributes = symbol
    //         .GetAttributes()
    //         .Select(a => new AttributeSig(a))
    //         .ToList();
    // }
    //
    // public SignatureAttributes(MemberInfo member)
    // {
    //     _attributes = member
    //         .GetCustomAttributesData()
    //         .Select(static a => new AttributeSig(a))
    //         .ToList();
    // }
    //
    // public bool HasAttribute<TAttribute>()
    //     where TAttribute : Attribute
    // {
    //     string attributeName = typeof(TAttribute).Name;
    //     return _attributes.Any(attr => attr.Name == attributeName);
    // }
    //
    // IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    //
    // public IEnumerator<AttributeSig> GetEnumerator()
    // {
    //     return _attributes
    //         .GetEnumerator();
    // }

    public override string ToString()
    {
        if (Count == 0) return string.Empty;
        return CodeBuilder.New
            .Append('[')
            .Delimit(", ", this, static (cb, attr) => cb.Append(attr))
            .Append(']')
            .ToStringAndDispose();
    }
}