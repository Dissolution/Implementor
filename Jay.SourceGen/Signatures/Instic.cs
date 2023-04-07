namespace Jay.SourceGen.Signatures;

[Flags]
public enum Instic
{
    Static = 1 << 0,

    Instance = 1 << 1,

    Any = Static | Instance,
}
