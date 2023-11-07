namespace Implementor.Reflection;

public static class KeywordsExtensions
{
    public static Keywords GetKeywords(this ISymbol? symbol)
    {
        Keywords keywords = new();
        AddKeywords(keywords, symbol);
        return keywords;
    }

    public static Keywords GetKeywords(this MemberInfo? member)
    {
        throw new NotImplementedException();
    }

    private static void AddKeywords(Keywords keywords, ISymbol? symbol)
    {
        if (symbol is null) return;
        
        if (symbol.IsAbstract)
            keywords.Add("abstract");
        if (symbol.IsSealed)
            keywords.Add("sealed");
        if (symbol.IsVirtual)
            keywords.Add("virtual");
        if (symbol.IsExtern)
            keywords.Add("extern");
        if (symbol.IsOverride)
            keywords.Add("override");
        
        switch (symbol)
        {
            case IFieldSymbol fieldSymbol:
            {
                if (fieldSymbol.IsConst)
                    keywords.Add("const");
                if (fieldSymbol.IsRequired)
                    keywords.Add("required");
                if (fieldSymbol.IsVolatile)
                    keywords.Add("volatile");
                if (fieldSymbol.IsReadOnly)
                    keywords.Add("readonly");
                return;
            }
            case IMethodSymbol methodSymbol:
            {
                if (methodSymbol.IsInitOnly)
                    keywords.Add("init");
                if (methodSymbol.IsReadOnly)
                    keywords.Add("readonly");
                if (methodSymbol.IsAsync)
                    keywords.Add("async");
                return;
            }
            case IEventSymbol eventSymbol:
            {
                AddKeywords(keywords, eventSymbol.AddMethod);
                AddKeywords(keywords, eventSymbol.RemoveMethod);
                AddKeywords(keywords, eventSymbol.RaiseMethod);
                return;
            }
            case IPropertySymbol propertySymbol:
            {
                AddKeywords(keywords, propertySymbol.GetMethod);
                AddKeywords(keywords, propertySymbol.SetMethod);
                return;
            }
            case ITypeSymbol typeSymbol:
            {
                if (typeSymbol.IsReadOnly)
                    keywords.Add("readonly");
                return;
            }
            default:
                return;
        }
    }
}