using Jay.SourceGen.Enums;

using System.Reflection.Metadata.Ecma335;

namespace Jay.SourceGen.Signatures;

[Flags]
public enum Instic
{
    Static = 1 << 0,

    Instance = 1 << 1,

    Any = Static | Instance,
}

public static class InsticExtensions
{
    public static Instic GetInstic(this ISymbol? symbol)
    {
        if (symbol is null) return default;
        if (symbol.IsStatic)
            return Instic.Static;
        return Instic.Instance;
    }

    public static Instic GetInstic(this MemberInfo? member)
    {
        switch (member)
        {
            case FieldInfo field:
            {
                return field.IsStatic ? Instic.Static : Instic.Instance;
            }
            case PropertyInfo property:
            {
                Instic instic;
                instic = GetInstic(property.GetMethod);
                if (instic != default) return instic;
                instic = GetInstic(property.SetMethod);
                return instic;
            }
            case EventInfo @event:
            {
                Instic instic;
                instic = GetInstic(@event.AddMethod);
                if (instic != default) return instic;
                instic = GetInstic(@event.RemoveMethod);
                return instic;
            }
            case MethodBase method:
            {
                return method.IsStatic ? Instic.Static : Instic.Instance;
            }
            case Type type:
            {
                return (type.IsAbstract && type.IsSealed) ? Instic.Static : Instic.Instance;
            }
            default:
                return default;
        }
    }
}