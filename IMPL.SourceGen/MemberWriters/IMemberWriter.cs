using Jay.SourceGen.Signatures;
using Jay.SourceGen.Text;

using System.Reflection;

namespace IMPL.SourceGen.MemberWriters;

public interface IMemberWriter
{
    bool CanWriteMember(MemberSig member);
    void WriteMember(IWorld world, MemberSig member, CodeBuilder code);
}

public readonly record struct SigWriter(MemberSig MemberSig, IMemberWriter MemberWriter);


public interface IWorld
{
    IReadOnlyList<IInterfaceGenerator> InterfaceGenerators { get; }
    IReadOnlyList<IMemberGenerator> MemberGenerators { get; }
    IReadOnlyList<IMemberWriter> MemberWriters { get; }
    IReadOnlyDictionary<MemberSig, IMemberWriter?> Members { get; }
}

public interface IMemberGenerator
{
    bool CanGenerateMember(MemberSig member);
    IEnumerable<SigWriter> GetWriters(MemberSig member);
}

public interface IInterfaceGenerator
{
    bool CanGenerateInterface(TypeSig interfaceType);
    IEnumerable<IMemberGenerator> GetGenerators(TypeSig interfaceType);
}


public class BackingFieldWriter : IMemberWriter
{
    public bool CanWriteMember(MemberSig member)
    {
        return member is FieldSig && member.Name.StartsWith("_");
    }

    public void WriteMember(IWorld world, MemberSig member, CodeBuilder code)
    {
        
    }
}


public class PropertyGenerator : IMemberGenerator
{
    private static (string Name, string FullName) GetBackingFieldNames(PropertySig property)
    {
        ReadOnlySpan<char> pFullName = property.FullName.AsSpan();
        Span<char> nameBuffer = stackalloc char[pFullName.Length + 1];
        nameBuffer[0] = '_';
        nameBuffer[1] = char.ToLower(pFullName[0]);
        TextHelper.CopyTo(pFullName[1..], nameBuffer[2..]);
        string name = nameBuffer.ToString();
        
        string fullName;
        var i = pFullName.LastIndexOf('.');
        if (i >= 0)
        {
            Span<char> fullNameBuffer = stackalloc char[i + 1 + name.Length];
            TextHelper.CopyTo(pFullName[..i], fullNameBuffer);
            fullNameBuffer[i] = '.';
            TextHelper.CopyTo(name, fullNameBuffer[(i+1)..]);
            fullName = fullNameBuffer.ToString();
        }
        else
        {
            fullName = name;
        }
        return (name, fullName);
    }


    public bool CanGenerateMember(MemberSig member)
    {
        return member is PropertySig;
    }

    public IEnumerable<SigWriter> GetWriters(MemberSig member)
    {
        var property = (PropertySig)member!;

        // Field
        Keywords fieldKeywords = default;
        if (!property.HasGet && !property.HasSet)
            fieldKeywords |= Keywords.Readonly;
        var names = GetBackingFieldNames(property);
        FieldSig backingField = new FieldSig
        {
            MemberType = MemberTypes.Field,
            Access = Accessibility.Private,
            Instic = Instic.Instance,
            Keywords = fieldKeywords,
            Name = names.Name,
            FullName = names.FullName,
        };

        yield return new SigWriter()
    }
}