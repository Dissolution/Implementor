using Microsoft.CodeAnalysis.Text;

using System.Text;

namespace Jay.SourceGen;

public readonly record struct SourceCode(string FileName, SourceText Code)
{
    public SourceCode(string hintName, string code)
        : this(hintName, SourceText.From(code, Encoding.UTF8))
    {

    }
}
