namespace Jay.SourceGen.Text;

public enum Naming
{
    /// <summary>
    /// Default / No Changes
    /// </summary>
    /// <remarks>
    /// "membername" -> "membername"<br/>
    /// "memberName" -> "memberName"<br/>
    /// "MemberName" -> "MemberName"<br/>
    /// "MEMBERNAME" -> "MEMBERNAME"<br/>
    /// </remarks>
    Default = 0,

    /// <summary>
    /// Lowercase
    /// </summary>
    /// <remarks>
    /// "membername" -> "membername"<br/>
    /// "memberName" -> "membername"<br/>
    /// "MemberName" -> "membername"<br/>
    /// "MEMBERNAME" -> "membername"<br/>
    /// </remarks>
    Lower,

    /// <summary>
    /// Uppercase
    /// </summary>
    /// <remarks>
    /// "membername" -> "MEMBERNAME"<br/>
    /// "memberName" -> "MEMBERNAME"<br/>
    /// "MemberName" -> "MEMBERNAME"<br/>
    /// "MEMBERNAME" -> "MEMBERNAME"<br/>
    /// </remarks>
    Upper,

    /// <summary>
    /// Camelcase
    /// </summary>
    /// <remarks>
    /// "membername" -> "membername"<br/>
    /// "memberName" -> "memberName"<br/>
    /// "MemberName" -> "memberName"<br/>
    /// "MEMBERNAME" -> "mEMBERNAME"<br/>
    /// </remarks>
    Camel,

    /// <summary>
    /// Pascalcase
    /// </summary>
    /// <remarks>
    /// "membername" -> "Membername"<br/>
    /// "memberName" -> "MemberName"<br/>
    /// "MemberName" -> "MemberName"<br/>
    /// "MEMBERNAME" -> "MEMBERNAME"<br/>
    /// </remarks>
    Pascal,

    ///// <summary>
    ///// Title Case
    ///// </summary>
    ///// <remarks>
    ///// <see cref="TextInfo.ToTitleCase(string)"/>
    ///// </remarks>
    //Title,

    /// <summary>
    /// Snake Case
    /// </summary>
    /// <remarks>
    /// "MemberName" => "member_name"
    /// "NameOfFBI" => "name_of_fbi"
    /// </remarks>
    Snake,

    /// <summary>
    /// C# private field naming convention
    /// </summary>
    /// <remarks>
    /// "membername" -> "_membername"<br/>
    /// "memberName" -> "_memberName"<br/>
    /// "MemberName" -> "_memberName"<br/>
    /// "MEMBERNAME" -> "_mEMBERNAME"<br/>
    /// </remarks>
    Field,

    /// <summary>
    /// Variable naming <br/>
    /// Ensures the output is a valid variable name
    /// </summary>
    Variable,
}

public static class NamingExtensions
{
    [return: NotNullIfNotNull(nameof(text))]
    public static string? WithNaming(this string? text, Naming naming)
    {
        if (text is null) return null;
        switch (naming)
        {
            case Naming.Lower:
                return text.ToLower();
            case Naming.Upper:
                return text.ToUpper();
            case Naming.Variable:
            {
                return text.ToVariableName();
            }
            case Naming.Camel:
            {
                int len = text.Length;

                if (len == 0) return "";
                if (char.IsLower(text[0])) return text;

                Span<char> nameBuffer = stackalloc char[len];
                nameBuffer[0] = char.ToLower(text[0]);
                TextHelper.CopyTo(text.AsSpan(1), nameBuffer.Slice(1));
                return nameBuffer.ToString();
            }
            case Naming.Pascal:
            {
                int len = text.Length;

                if (len == 0) return "";
                if (char.IsUpper(text[0])) return text;

                Span<char> nameBuffer = stackalloc char[len];
                nameBuffer[0] = char.ToUpper(text[0]);
                TextHelper.CopyTo(text.AsSpan(1), nameBuffer.Slice(1));
                return nameBuffer.ToString();
            }
            case Naming.Snake:
            {
                int textLen = text.Length;
                if (textLen < 2) return text;

                Span<char> nameBuffer = stackalloc char[textLen * 2]; // way large
                int n = 0;

                bool prevWasCaps = false;
                int textIndex = 0;

                // First char is just lower
                char ch = text[textIndex];
                prevWasCaps = char.IsUpper(ch);
                nameBuffer[n++] = char.ToLower(ch);

                // Process everything else using the usual rules
                for (; textIndex < textLen; textIndex++)
                {
                    ch = text[textIndex];
                    if (char.IsUpper(ch))
                    {
                        // start a new segment so long as we're not part of a possible acronym
                        if (!prevWasCaps)
                        {
                            nameBuffer[n++] = '_';
                        }
                        nameBuffer[n++] = char.ToLower(ch);
                        prevWasCaps = true;
                    }
                    else
                    {
                        nameBuffer[n++] = ch;
                        prevWasCaps = false;
                    }
                }

                return nameBuffer.Slice(0, n).ToString();
            }
            case Naming.Field:
            {
                int len = text.Length;
                if (len == 0) return "";
                Span<char> nameBuffer = stackalloc char[len + 1];
                nameBuffer[0] = '_';
                nameBuffer[1] = char.ToLower(text[0]);
                TextHelper.CopyTo(text.AsSpan(1), nameBuffer.Slice(2));
                return nameBuffer.ToString();
            }
            case Naming.Default:
            default:
                return text;
        }
    }
}