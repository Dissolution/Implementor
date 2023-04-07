using Jay.SourceGen.Coding;
using Jay.SourceGen.Enums;

using System.Reflection;

[Flags]
public enum Keywords
{
    None = 0,
    Virtual = 1 << 0,
    Abstract = 1 << 1,
    Sealed = 1 << 2,
    Partial = 1 << 3,
    Readonly = 1 << 4,
}

internal static class KeywordsExtensions
{
    public static void DeclareTo(this Keywords keywords, CodeBuilder codeBuilder)
    {
        codeBuilder.Enumerate(
            keywords.GetFlags(),
            static (cb, v) => cb.Append(v.ToString().ToLower()).Append(' '));
    }
}

public static class KeywordUtil
{
    public static Keywords FromSymbol(ISymbol symbol)
    {
        Keywords keywords = default;
        if (symbol.IsVirtual)
            keywords |= Keywords.Virtual;
        if (symbol.IsAbstract)
            keywords |= Keywords.Abstract;
        if (symbol.IsSealed)
            keywords |= Keywords.Sealed;
        if (symbol is IFieldSymbol fieldSymbol)
        {
            if (fieldSymbol.IsReadOnly)
                keywords |= Keywords.Readonly;
        }
        return keywords;
    }

    public static Keywords FromMember(MemberInfo? member)
    {
        Keywords keywords = default;
        switch (member)
        {
            case null: 
                break;
            case FieldInfo field:
            {
                if (field.IsInitOnly)
                    keywords |= Keywords.Readonly;
                break;
            }
            case PropertyInfo property:
            {
                keywords |= FromMember(property.GetMethod);
                keywords |= FromMember(property.SetMethod);
                break;
            }
            case EventInfo @event:
            {
                keywords |= FromMember(@event.AddMethod);
                keywords |= FromMember(@event.RemoveMethod);
                break;
            }
            case MethodBase method:
            {
                if (method.IsVirtual)
                    keywords |= Keywords.Virtual;
                if (method.IsAbstract)
                    keywords |= Keywords.Abstract;
                break;
            }
            default:
                throw new ArgumentException("", nameof(member));
        }
        return keywords;
    }

    public static void Write(Keywords keywords, CodeBuilder codeBuilder)
    {
        var flags = keywords.GetFlags();
        codeBuilder.Delimit(" ", flags, static (cb,f) => cb.AppendValue(f, "lc"));
    }
}
