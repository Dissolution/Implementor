namespace Jay.SourceGen.Coding;

[Flags]
public enum Visibility
{
    None = 0,
    Instance = 1 << 0,
    Static = 1 << 1,
    Private = 1 << 2,
    Protected = 1 << 3,
    Internal = 1 << 4,
    Public = 1 << 5,
}

public static class VisibilityExtensions
{
    public static Visibility ToVisibility(this Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.NotApplicable => default,
            Accessibility.Private => Visibility.Private,
            Accessibility.ProtectedAndInternal => Visibility.Protected | Visibility.Internal,
            Accessibility.Protected => Visibility.Protected,
            Accessibility.Internal => Visibility.Internal,
            Accessibility.ProtectedOrInternal => Visibility.Protected | Visibility.Internal,
            Accessibility.Public => Visibility.Public,
            _ => throw new ArgumentException("Invalid Accessibility", nameof(accessibility)),
        };
    }

    public static Visibility ToVisibility(this BindingFlags bindingFlags)
    {
        Visibility visibility = default;
        if (bindingFlags.HasFlag(BindingFlags.Instance))
            visibility |= Visibility.Instance;
        if (bindingFlags.HasFlag(BindingFlags.Static))
            visibility |= Visibility.Static;
        if (bindingFlags.HasFlag(BindingFlags.Public))
            visibility |= Visibility.Public;
        if (bindingFlags.HasFlag(BindingFlags.NonPublic))
            visibility |= (Visibility.Private | Visibility.Protected | Visibility.Internal);
        return visibility;
    }

    public static Visibility GetVisibility(this ISymbol? symbol)
    {
        if (symbol is null) return default;
        var visibility =  symbol.DeclaredAccessibility.ToVisibility();
        if (symbol.IsStatic)
            visibility |= Visibility.Static;
        else
            visibility |= Visibility.Instance;
        return visibility;
    }

    public static Visibility GetVisibility(this MemberInfo? member)
    {
        Visibility visibility = default;
        switch (member)
        {
            case null: 
                return visibility;
            case FieldInfo field:
            {
                if (field.IsPrivate)
                    visibility |= Visibility.Private;
                if (field.IsFamily)
                    visibility |= Visibility.Protected;
                if (field.IsFamilyAndAssembly)
                    visibility |= (Visibility.Protected | Visibility.Internal);
                if (field.IsFamilyOrAssembly)
                    visibility |= (Visibility.Protected | Visibility.Internal);
                if (field.IsAssembly)
                    visibility |= Visibility.Internal;
                if (field.IsPublic)
                    visibility |= Visibility.Public;
                if (field.IsStatic)
                    visibility |= Visibility.Static;
                else
                    visibility |= Visibility.Instance;
                return visibility;
            }
            case PropertyInfo property:
            {
                visibility |= GetVisibility(property.GetMethod);
                visibility |= GetVisibility(property.SetMethod);
                return visibility;
            }
            case EventInfo @event:
            {
                visibility |= GetVisibility(@event.AddMethod);
                visibility |= GetVisibility(@event.RemoveMethod);
                return visibility;
            }
            case MethodBase method:
            {
                if (method.IsPrivate)
                    visibility |= Visibility.Private;
                if (method.IsFamily)
                    visibility |= Visibility.Protected;
                if (method.IsFamilyAndAssembly)
                    visibility |= (Visibility.Protected | Visibility.Internal);
                if (method.IsFamilyOrAssembly)
                    visibility |= (Visibility.Protected | Visibility.Internal);
                if (method.IsAssembly)
                    visibility |= Visibility.Internal;
                if (method.IsPublic)
                    visibility |= Visibility.Public;
                if (method.IsStatic)
                    visibility |= Visibility.Static;
                else
                    visibility |= Visibility.Instance;
                return visibility;
            }
            case Type type:
            {
                if (type.IsPublic)
                    visibility |= Visibility.Public;
                if (type.IsAbstract && type.IsSealed)
                    visibility |= Visibility.Static;
                else
                    visibility |= Visibility.Instance;
                return visibility;
            }
            default:
                throw new ArgumentException("", nameof(member));
        }
    }
}