using Implementor.Reflection;

namespace Implementor.Text;

public static class ToCodeHelper
{
    private static readonly Dictionary<Type, Delegate?> _cache = new();

    static ToCodeHelper()
    {
        AddToCode<Visibility>(static (cb, vis) =>
        {
            bool wrote = false;
            if (vis.HasFlag(Visibility.Static))
            {
                cb.Append("static ");
                wrote = true;
            }
            if (vis.HasFlag(Visibility.Private))
            {
                cb.Append("private ");
                wrote = true;
            }
            if (vis.HasFlag(Visibility.Protected))
            {
                cb.Append("protected ");
                wrote = true;
            }
            if (vis.HasFlag(Visibility.Internal))
            {
                cb.Append("internal ");
                wrote = true;
            }
            if (vis.HasFlag(Visibility.Public))
            {
                cb.Append("public ");
                wrote = true;
            }

            if (wrote)
            {
                cb.TryRemove(^1..);
            }
            return wrote;
        });
    }

    public static void AddToCode<T>(CBVP<T> writeValueTo)
    {
        _cache[typeof(T)] = writeValueTo;
    }
    
    public static bool WriteValueTo<T>([AllowNull, NotNullWhen(true)] T value, CodeBuilder codeBuilder)
    {
        if (value is null) 
            return false;
        if (value is IToCode)
            return ((IToCode)value).WriteTo(codeBuilder);
        if (_cache.TryGetValue(typeof(T), out var @delegate) &&
            @delegate is CBVP<T> build)
        {
            return build(codeBuilder, value);
        }

        if (value is Enum e)
        {
            throw new NotImplementedException();
        }

        string? str;
        if (value is IFormattable)
        {
            str = ((IFormattable)value).ToString(null, null);
        }
        else
        {
            str = value.ToString();
        }

        if (string.IsNullOrEmpty(str))
            return false;

        codeBuilder.Append(str);
        return true;
    }
}