namespace Jay.SourceGen.Enums;

[Flags]
public enum Visibility
{
    Private = 1 << 0,
    Protected = 1 << 1,
    Internal = 1 << 2,
    Public = 1 << 3,
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

    public static Visibility GetVisibility(this ISymbol? symbol)
    {
        if (symbol is null) return default;
        return symbol.DeclaredAccessibility.ToVisibility();
    }

    public static Visibility GetVisibility(this MemberInfo? member)
    {
        Visibility visibility = default;
        switch (member)
        {
            case null: return visibility;
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
                return visibility;
            }
            case Type type:
            {
                if (type.IsPublic)
                    visibility |= Visibility.Public;
                return visibility;
            }
            default:
                throw new ArgumentException("", nameof(member));
        }
    }
}
