using System;
using System.Collections.Generic;
using System.Text;

namespace IMPL.SourceGen.Signatures;

public abstract class MemberSig
{
    public INamedTypeSymbol MemberSymbol { get; }



    protected MemberSig()
    {

    }
}

public class TypeSig : MemberSig
{
    public TypeSig(ITypeSymbol typeSymbol)
}
