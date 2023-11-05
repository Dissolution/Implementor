using Implementor.Reflection;

namespace Implementor.Text;

/// <summary>
/// This value can be written as C# code to a <see cref="CodeBuilder"/>
/// </summary>
public interface IToCode
{
    /// <summary>
    /// Writes a C# code representation of this value to the given <paramref name="codeBuilder"/>
    /// </summary>
    /// <param name="codeBuilder">The <see cref="CodeBuilder"/> to write to</param>
    /// <returns>
    /// <c>true</c> if we wrote anything; otherwise, <c>false</c>
    /// </returns>
    bool WriteTo(CodeBuilder codeBuilder);
}

public static class ToCodeHelper
{
    private delegate bool WriteValueDelegate<in T>([AllowNull, NotNullWhen(true)] T value, CodeBuilder codeBuilder);

    private static Dictionary<Type, Delegate?> _cache = new();

    static ToCodeHelper()
    {
        _cache = new();
        _cache[typeof(Visibility)] = (Delegate)(WriteValueDelegate<Visibility>)((v, cb) =>
        {
            cb.Append(v, Casing.Lower);
            return true;
        });
    }
    
    public static bool WriteValueTo<T>([AllowNull, NotNullWhen(true)] T value, CodeBuilder codeBuilder)
    {
        string? str;
        switch (value)
        {
            case null:
                return false;
            case IToCode:
                return ((IToCode)value).WriteTo(codeBuilder);
            case Enum e:
            {
                codeBuilder.Append<Enum>(e, Casing.Lower);
                return true;
            }
            case IFormattable:
            {
                str = ((IFormattable)value).ToString(null, null);
                break;
            }
            default:
            {
                str = value.ToString();
                break;
            }
        }

        if (string.IsNullOrEmpty(str))
            return false;

        codeBuilder.Append(str);
        return true;
    }
}