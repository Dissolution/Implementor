using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Jay.SourceGen.Enums;

namespace Jay.SourceGen.Extensions;

public static class MemberInfoExtensions
{
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
            default:
                throw new ArgumentException("", nameof(member));
        }
    }

    public static bool IsStatic(this MemberInfo? member)
    {
        switch (member)
        {
            case null: return false;
            case FieldInfo field:
            {
                return field.IsStatic;
            }
            case PropertyInfo property:
            {
                return IsStatic(property.GetMethod) || IsStatic(property.SetMethod);
            }
            case EventInfo @event:
            {
                return IsStatic(@event.AddMethod) || IsStatic(@event.RemoveMethod);
            }
            case MethodBase method:
            {
                return method.IsStatic;
            }
            default:
                throw new ArgumentException("", nameof(member));
        }
    }

}
