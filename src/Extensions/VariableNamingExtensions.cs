﻿namespace Implementor.Extensions;

public static class VariableNamingExtensions
{
    private static readonly ImmutableHashSet<string> _keywords = SyntaxFacts
               .GetKeywordKinds()
               .Select(SyntaxFacts.GetText)
               .ToImmutableHashSet();

    private static int _counter = 0;

    public static string GetVariableName(this ITypeSymbol typeSymbol)
    {
        // Account for Interface 'IXyz' to 'ixyz' rather than 'iXyz'
        var typeName = typeSymbol.Name;
        int typeNameLen = typeName.Length;
        if (typeNameLen == 0)
        {
            // Weird, but be sure we don't ever return the same
            return $"__{Interlocked.Increment(ref _counter)}";
        }
        int n = 0;

        string varName;

        char ch = typeName[n++];
        if (char.IsUpper(ch))
        {
            Span<char> buffer = stackalloc char[typeNameLen];
            int b = 0;

            if (ch == 'I')
            {
                buffer[b++] = 'i';
                while (n < typeNameLen)
                {
                    ch = typeName[n];
                    if (char.IsLower(ch)) break;
                    buffer[b++] = char.ToLower(ch);
                    n++;
                }
                if (n < typeNameLen)
                {
                    
                    typeName.AsSpan(n).CopyTo(buffer[b..]);
                }
            }
            else
            {
                buffer[b++] = char.ToLower(ch);
                typeName.AsSpan(1).CopyTo(buffer[b..]);
            }

            varName = buffer.ToString();
        }
        else
        {
            varName = typeName;
        }

        // Check if we have to escape the name
        if (!SyntaxFacts.IsValidIdentifier(varName) || _keywords.Contains(varName))
        {
            return $"@{varName}";
        }
        return varName;


    }

    public static string ToVariableName(this string? name) => ToVariableName(name.AsSpan());

    public static string ToVariableName(this ReadOnlySpan<char> name)
    {
        int nameLen = name.Length;
        if (nameLen == 0) return "_";

        // Convert to camel-case
        Span<char> buffer = stackalloc char[nameLen];
        buffer[0] = char.ToLower(name[0]);
        name[1..].CopyTo(buffer[1..]);
        string varName = buffer.ToString();
        // Check if we have to escape the name
        if (!SyntaxFacts.IsValidIdentifier(varName) || _keywords.Contains(varName))
        {
            return $"@{varName}";
        }
        return varName;
    }
}