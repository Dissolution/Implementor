// using Implementor.Utilities;
//
// namespace Implementor.Reflection;
//
// public abstract class Sig : IEquatable<Sig>
// {
//     public static bool operator ==(Sig? left, Sig? right) => EqualityComparer<Sig>.Default.Equals(left!, right!);
//     public static bool operator !=(Sig? left, Sig? right) => !(left == right);
//     
//     public static bool TryCreate([AllowNull, NotNullWhen(true)] object? obj, [NotNullWhen(true)] out Sig? signature)
//     {
//         switch (obj)
//         {
//             case IFieldSymbol fieldSymbol:
//                 signature = new FieldSig(fieldSymbol);
//                 return true;
//             case FieldInfo fieldInfo:
//                 signature = new FieldSig(fieldInfo);
//                 return true;
//             case IPropertySymbol propertySymbol:
//                 signature = new PropertySig(propertySymbol);
//                 return true;
//             case PropertyInfo propertyInfo:
//                 signature = new PropertySig(propertyInfo);
//                 return true;
//             case IEventSymbol eventSymbol:
//                 signature = new EventSig(eventSymbol);
//                 return true;
//             case EventInfo eventInfo:
//                 signature = new EventSig(eventInfo);
//                 return true;
//             case IMethodSymbol methodSymbol:
//                 signature = new MethodSig(methodSymbol);
//                 return true;
//             case MethodBase methodBase:
//                 signature = new MethodSig(methodBase);
//                 return true;
//             case Type type:
//                 signature = new TypeSig(type);
//                 return true;
//             case IParameterSymbol parameterSymbol:
//                 signature = new ParameterSig(parameterSymbol);
//                 return true;
//             case ParameterInfo parameterInfo:
//                 signature = new ParameterSig(parameterInfo);
//                 return true;
//             case AttributeData attributeData:
//                 signature = new AttributeSig(attributeData);
//                 return true;
//             case CustomAttributeData customAttrData:
//                 signature = new AttributeSig(customAttrData);
//                 return true;
//             default:
//                 signature = default;
//                 return false;
//         }
//     }
//
//     public string? Name { get; set; } = null;
//     public Visibility Visibility { get; set; } = Visibility.None;
//     public Keywords Keywords { get; } = new();
//
//     public virtual bool Equals(Sig? signature)
//     {
//         return signature is not null &&
//             string.Equals(this.Name, signature.Name) &&
//             this.Visibility == signature.Visibility &&
//             this.Keywords.SetEquals(signature.Keywords);
//     }
//     
//     public override bool Equals(object? obj)
//     {
//         return TryCreate(obj, out var signature) && Equals(signature);
//     }
//
//     /// <summary>
//     /// <b>WARNING</b>: <see cref="Sig"/> and derivatives are mutable!
//     /// </summary>
//     public override int GetHashCode()
//     {
//         return Hasher.Combine(GetType(), Name, Visibility, Keywords);
//     }
// }