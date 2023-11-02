namespace Implementor.SourceGen.Modifiers;

public interface IInterfaceImplementationModifier : IImplModifier
{
    bool AppliesTo(TypeSig interfaceType);
}
