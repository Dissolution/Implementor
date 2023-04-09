//namespace IMPL.SourceGen.MemberWriters;

//internal class SpecGen
//{
//    public static SourceCode DoThing(ImplSpec spec)
//    {
//        var implDeets = new ImplDeets();

//        dynamic fieldWriter = default;
//        PropertyWriter propertyWriter = default!;
//        dynamic eventWriter = default; // ???
//        // Process interfaces to adjust behavior
//        foreach (var interfaceType in spec.InterfaceTypes)
//        {
//            // Do we have anything that handles this interface?
//        }

//        throw new NotImplementedException();
//    }


//}


//public class ImplDeets
//{
//    public TypeSig Type { get; }
//    public List<(MemberSig, IMemberWriter?)> MemberWriters { get; }
//}

//public class Implementer
//{
//    private IMemberWriter _fieldWriter;
    
//}


//public interface IMemberWriter
//{
//    void Write(CodeBuilder codeBuilder);
//}

//public abstract class MemberWriter : IMemberWriter
//{
//    public abstract void Write(CodeBuilder codeBuilder);
//}

//public abstract class MemberSigWriter<TMember> : MemberWriter
//    where TMember : MemberSig
//{
//    public TMember Member { get; }

//    public MemberSigWriter(TMember member)
//    {
//        this.Member = member;
//    }
//}


//public class CustomMemberWriter : MemberWriter
//{
//    protected readonly Action<CodeBuilder> _writeMember;

//    public CustomMemberWriter(Action<CodeBuilder> writeMember)
//    {
//        _writeMember = writeMember;
//    }

//    public override void Write(CodeBuilder codeBuilder)
//    {
//        _writeMember?.Invoke(codeBuilder);
//    }
//}

//public interface IMemberImplementer
//{
//    IEnumerable<IMemberWriter> GetRelatedWriters(MemberSig memberSig);
//}

//public abstract class MemberSigImplementer<TMember> : IMemberImplementer
//    where TMember : MemberSig
//{
//    IEnumerable<IMemberWriter> IMemberImplementer.GetRelatedWriters(MemberSig memberSig)
//    {
//        if (memberSig is TMember member)
//        {
//            return GetWriters(member);
//        }
//        throw new ArgumentException($"{memberSig} is not a {typeof(TMember)}", nameof(memberSig));
//    }

//    public abstract IEnumerable<IMemberWriter> GetWriters(TMember member);
//}


//public abstract class PropertyWriter : MemberSigWriter<PropertySig>
//{
//    public PropertyWriter(PropertySig property)
//        : base(property) { }
//}


//public class PropertyFieldImplementer : MemberSigImplementer<PropertySig>
//{
//    protected virtual string GetBackingFieldName(PropertySig property)
//    {
//        ReadOnlySpan<char> pFullName = property.FullName.AsSpan();
//        Span<char> nameBuffer = stackalloc char[pFullName.Length + 1];
//        nameBuffer[0] = '_';
//        nameBuffer[1] = char.ToLower(pFullName[0]);
//        TextHelper.CopyTo(pFullName[1..], nameBuffer[2..]);
//        string name = nameBuffer.ToString();
//        return name;
//    }


//    public override IEnumerable<IMemberWriter> GetRelatedWriters(PropertySig property)
//    {
//        // Backing Field
//        FieldSig backingField = new FieldSig
//        {
//            Name = GetBackingFieldName(property),
//            FieldType = property.PropertyType,
//            Visibility = Visibility.Private,
//            Instic = Instic.Instance,
//        };
//        // We can make it readonly if we have no setter
//        if (property.SetMethod is null)
//        {
//            backingField.Keywords |= Keywords.Readonly;
//        }

//        yield return new FieldWriter(backingField);

//        // Property itself

//    }
//}

//public abstract class PropertyFieldWriter : PropertyWriter
//{
//    protected virtual (string Name, string FullName) GetBackingFieldNames(PropertySig property)
//    {
//        ReadOnlySpan<char> pFullName = property.FullName.AsSpan();
//        Span<char> nameBuffer = stackalloc char[pFullName.Length + 1];
//        nameBuffer[0] = '_';
//        nameBuffer[1] = char.ToLower(pFullName[0]);
//        TextHelper.CopyTo(pFullName[1..], nameBuffer[2..]);
//        string name = nameBuffer.ToString();

//        string fullName;
//        var i = pFullName.LastIndexOf('.');
//        if (i >= 0)
//        {
//            Span<char> fullNameBuffer = stackalloc char[i + 1 + name.Length];
//            TextHelper.CopyTo(pFullName[..i], fullNameBuffer);
//            fullNameBuffer[i] = '.';
//            TextHelper.CopyTo(name, fullNameBuffer[(i + 1)..]);
//            fullName = fullNameBuffer.ToString();
//        }
//        else
//        {
//            fullName = name;
//        }
//        return (name, fullName);
//    }

//    public override void AddRelatedMembers(PropertySig property, ImplDeets implDeets)
//    {
//        // We have a backing field
//        var names = GetBackingFieldNames(property);
//        FieldSig backingField = new FieldSig
//        {
//            Name = names.Name,
//            FullName = names.FullName,
//        };
//        // We can make it readonly if we have no setter
//        if (property.SetMethod is null)
//        {
//            backingField.Keywords |= Keywords.Readonly;
//        }
//        // We want to write this backing field, we will need to use it
//        implDeets.MemberWriters.Add((backingField, FieldWriter.Default));
//    }
//}

//public class PropertySetFieldEqualsWriter : PropertyFieldWriter
//{
//    public List<CBA> PreValueSetActions { get; } = new(0);
//    public List<CBA> PostValueSetActions { get; } = new(0);

//    protected internal bool SetField<T>(ref T field, T value, bool force = false, [CallerMemberName] string? propertyName = null)
//    {
//        if (force || !EqualityComparer<T>.Default.Equals(field, value))
//        {
//            // pres
//            field = value;
//            // posts
//            return true;
//        }
//        return false;
//    }

//    public override void AddRelatedMembers(PropertySig property, ImplDeets implDeets)
//    {
//        base.AddRelatedMembers(property, implDeets);

//        // We need SetField
//        MethodSig setFieldMethod = new MethodSig()
//        {
//            Name = "SetField",
//            ReturnType = typeof(void),
//            Parameters = new ParameterSig[]
//            {
//                new ParameterSig
//                {
//                    Name = "field"
//                }
//            },
//        };

//        void writeSetFieldMethod(MethodSig method, CodeBuilder codeBuilder)
//        {

//        }


//        var cmw = new CustomMethodWriter(writeSetFieldMethod);
//        implDeets.MemberWriters.Add((setFieldMethod, cmw));


//    }

//    public override void Write(PropertySig property, CodeBuilder codeBuilder)
//    {
//        // We know we created a backing field
//        var names = GetBackingFieldNames(property);

//        codeBuilder
//          .AppendValue(property.Visibility, "lc")
//          .AppendIf(property.Instic == Instic.Instance, " ", " static ")
//          .AppendKeywords(property.Keywords)
//          .AppendLine(property.Name)
//          .BracketBlock(propBlock =>
//          {
//              // Getter?
//              if (property.GetMethod is not null)
//              {
//                  propBlock.CodeLine($"get => this.{names.Name};");
//              }
//              // Setter?
//              var setMethod = property.SetMethod;
//              if (setMethod is not null)
//              {
//                  if (setMethod.Keywords.HasFlag(Keywords.Init))
//                  {
//                      propBlock.AppendLine("init");

//                  }
//                  else
//                  {
//                      propBlock.AppendLine("set");
//                  }
//                  propBlock.BracketBlock(setBlock =>
//                  {

//                  })
//              }
//          }).NewLine();
//    }
//}
