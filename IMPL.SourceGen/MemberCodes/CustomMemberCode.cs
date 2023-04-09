﻿using IMPL.SourceGen.MemberWriters;

namespace IMPL.SourceGen.MemberCodes;

public sealed class CustomMemberCode : IMemberCode
{
    private readonly Action<Implementer, CodeBuilder> _writeCode;

    public MemberPos Pos {get; }

    public CustomMemberCode(MemberPos pos, Action<Implementer, CodeBuilder> writeCode)
    {
        this.Pos = pos;
        _writeCode = writeCode;
    }

    public void Write(Implementer implementer, CodeBuilder codeBuilder)
    {
        _writeCode?.Invoke(implementer, codeBuilder);
    }
}
