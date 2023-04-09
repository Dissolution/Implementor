namespace IMPL.SourceGen.MemberWriters;

public interface IImplModifier
{
    void PreRegister(Implementer implementer);
}

public interface IInterfaceImplModifer : IImplModifier
{
    bool AppliesTo(TypeSig interfaceType);
}
