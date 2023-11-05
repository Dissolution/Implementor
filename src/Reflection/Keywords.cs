using Implementor.Text;
using Implementor.Utilities;

namespace Implementor.Reflection;

public sealed class Keywords : HashSet<string>, IToCode
{
    public Keywords() : base(StringComparer.OrdinalIgnoreCase) { }

    public bool WriteTo(CodeBuilder codeBuilder)
    {
        if (this.Count == 0)
            return false;
        codeBuilder.Delimit(" ", this, static (cb, kw) => cb.Append(kw, Casing.Lower));
        return true;
    }

    public override bool Equals(object? obj)
    {
        return obj is Keywords keywords && this.SetEquals(keywords);
    }

    public override int GetHashCode()
    {
        return Hasher.Combine<string>(this);
    }

    public override string ToString()
    {
        return CodeBuilder.New
            .Delimit(static b => b.Append(' '), this, static (cb, keyword) => cb.Append(keyword, Casing.Lower))
            .ToStringAndDispose();
    }
}