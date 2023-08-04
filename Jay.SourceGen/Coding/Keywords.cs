namespace Jay.SourceGen.Coding;

[Flags]
public enum Keywords
{
    None = 0,
    
    Abstract = 1<<0,
    Virtual = 1<<1,
    Sealed = 1<<2,
    Override = 1<<3,
    
    ReadOnly = 1<<4,
    Required = 1<<5,
    Volatile = 1<<6,
    
    WriteOnly = 1<<7,
    
    Async = 1<<8,
}

public static class KeywordsExtensions
{
    public static Keywords GetKeywords(this ISymbol? symbol)
    {
        Keywords keywords = default;
        if (symbol is null) return keywords;
        if (symbol.IsAbstract)
            keywords |= Keywords.Abstract;
        if (symbol.IsVirtual)
            keywords |= Keywords.Virtual;
        if (symbol.IsSealed)
            keywords |= Keywords.Sealed;
        if (symbol.IsOverride)
            keywords |= Keywords.Override;

        if (symbol is IFieldSymbol fieldSymbol)
        {
            if (fieldSymbol.IsReadOnly)
                keywords |= Keywords.ReadOnly;
            if (fieldSymbol.IsRequired)
                keywords |= Keywords.Required;
            if (fieldSymbol.IsVolatile)
                keywords |= Keywords.Volatile;
        }
        else if (symbol is IPropertySymbol propertySymbol)
        {
            if (propertySymbol.IsReadOnly)
                keywords |= Keywords.ReadOnly;
            if (propertySymbol.IsRequired)
                keywords |= Keywords.Required;
            if (propertySymbol.IsWriteOnly)
                keywords |= Keywords.WriteOnly;
        }
        else if (symbol is IEventSymbol eventSymbol)
        {

        }
        else if (symbol is IMethodSymbol methodSymbol)
        {
            if (methodSymbol.IsReadOnly)
                keywords |= Keywords.ReadOnly;
            if (methodSymbol.IsAsync)
                keywords |= Keywords.Async;
        }
        else if (symbol is ITypeSymbol typeSymbol)
        {
            throw new NotImplementedException();
        }
        else
        {
            throw new NotImplementedException();
        }
        return keywords;
    }
}