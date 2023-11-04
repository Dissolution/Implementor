using Implementor.Text;
using Implementor.Utilities;

namespace Implementor.Reflection;

public sealed class Keywords : HashSet<string>
{
    public Keywords() : base(StringComparer.OrdinalIgnoreCase) { }

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