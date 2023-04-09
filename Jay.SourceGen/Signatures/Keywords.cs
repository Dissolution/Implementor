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
    Init = 1 << 5,
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
    public static bool TryParse(string? input, out Keywords keywords)
    {
        keywords = default;
        if (string.IsNullOrEmpty(input)) return false;
        if (Enum.TryParse<Keywords>(input, true, out keywords))
            return true;

        var split = input!.Split(new char[]{' ', ',', '|'}, StringSplitOptions.RemoveEmptyEntries);
        foreach (var segment in split)
        {
            if (Enum.TryParse<Keywords>(segment.Trim(), true, out var keyword))
                keywords |= keyword;
            else
                return false;
        }
        return true;
    }

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
            case MethodInfo method:
            {
                if (method.IsVirtual)
                    keywords |= Keywords.Virtual;
                if (method.IsAbstract)
                    keywords |= Keywords.Abstract;
                
                // Look for init for property set methods
                var returnMods = method.ReturnParameter.GetRequiredCustomModifiers();
                if (returnMods.Contains(typeof(System.Runtime.CompilerServices.IsExternalInit)))
                {
                    keywords |= Keywords.Init;
                }           
                break;
            }
            default:
                throw new ArgumentException("", nameof(member));
        }
        return keywords;
    }

    public static CodeBuilder AppendKeywords(this CodeBuilder codeBuilder, Keywords keywords)
    {
        var flags = keywords.GetFlags();
        return codeBuilder.Enumerate(flags, static (cb, f) => cb.AppendValue(f, "lc").Append(' '));
    }
}
