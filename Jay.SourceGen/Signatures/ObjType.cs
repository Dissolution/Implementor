namespace Jay.SourceGen.Signatures;

[Flags]
public enum ObjType
{
    Struct = 1 << 0,

    Class = 1 << 1,

    Interface = 1 << 2 | Class,

    Any = Struct | Class | Interface,
}