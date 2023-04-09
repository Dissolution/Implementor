using Jay.SourceGen.Coding;
using Jay.SourceGen.Signatures;
using Jay.SourceGen.Text;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace IMPL.SourceGen.MemberWriters;

internal class SpecGen
{
    public static SourceCode DoThing(ImplSpec spec)
    {
        var implDeets = new ImplDeets();

        dynamic fieldWriter = default;
        PropertyWriter propertyWriter = default!;
        dynamic eventWriter = default; // ???
        // Process interfaces to adjust behavior
        foreach (var interfaceType in spec.InterfaceTypes)
        {
            // Do we have anything that handles this interface?
        }

        throw new NotImplementedException();
    }


}


public class ImplDeets
{
    public TypeSig Type { get; }
    public List<(MemberSig, IMemberWriter?)> MemberWriters { get; }
}


public interface IMemberWriter
{
    void AddRelatedMembers(MemberSig member, ImplDeets world);
    void Write(MemberSig member, CodeBuilder codeBuilder);
}

public abstract class MemberWriter<TMember> : IMemberWriter
    where TMember : MemberSig
{
    void IMemberWriter.AddRelatedMembers(MemberSig member, ImplDeets world)
    {
        if (member is TMember tMember)
        {
            AddRelatedMembers(tMember, world);
        }
        else
        {
            throw new ArgumentException($"{member} is not a {typeof(TMember)}", nameof(member));
        }
    }

    void IMemberWriter.Write(MemberSig member, CodeBuilder codeBuilder)
    {
        if (member is TMember tMember)
        {
            Write(tMember, codeBuilder);
        }
        else
        {
            throw new ArgumentException($"{member} is not a {typeof(TMember)}", nameof(member));
        }
    }

    public virtual void AddRelatedMembers(TMember member, ImplDeets world)
    {
        // By default, do nothing!
    }

    public abstract void Write(TMember member, CodeBuilder codeBuilder);
}

public abstract class MethodWriter : MemberWriter<MethodSig>
{

}

public sealed class CustomMethodWriter : MethodWriter
{
    private readonly Action<MethodSig, CodeBuilder> _writeAction;

    public CustomMethodWriter(Action<MethodSig, CodeBuilder> writeAction)
    {
        _writeAction = writeAction;
    }

    public override void Write(MethodSig member, CodeBuilder codeBuilder)
    {
        _writeAction?.Invoke(member, codeBuilder);
    }
}

public abstract class PropertyWriter : MemberWriter<PropertySig>
{

}

public abstract class PropertyFieldWriter : PropertyWriter
{
    protected virtual (string Name, string FullName) GetBackingFieldNames(PropertySig property)
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
            TextHelper.CopyTo(name, fullNameBuffer[(i + 1)..]);
            fullName = fullNameBuffer.ToString();
        }
        else
        {
            fullName = name;
        }
        return (name, fullName);
    }

    public override void AddRelatedMembers(PropertySig property, ImplDeets implDeets)
    {
        // We have a backing field
        var names = GetBackingFieldNames(property);
        FieldSig backingField = new FieldSig
        {
            Name = names.Name,
            FullName = names.FullName,
        };
        // We can make it readonly if we have no setter
        if (property.SetMethod is null)
        {
            backingField.Keywords |= Keywords.Readonly;
        }
        // We want to write this backing field, we will need to use it
        implDeets.MemberWriters.Add((backingField, FieldWriter.Default));
    }
}

public class PropertySetFieldEqualsWriter : PropertyFieldWriter
{
    public List<CBA> PreValueSetActions { get; } = new(0);
    public List<CBA> PostValueSetActions { get; } = new(0);

    protected internal bool SetField<T>(ref T field, T value, bool force = false, [CallerMemberName] string? propertyName = null)
    {
        if (force || !EqualityComparer<T>.Default.Equals(field, value))
        {
            // pres
            field = value;
            // posts
            return true;
        }
        return false;
    }

    public override void AddRelatedMembers(PropertySig property, ImplDeets implDeets)
    {
        base.AddRelatedMembers(property, implDeets);

        // We need SetField
        MethodSig setFieldMethod = new MethodSig()
        {
            Name = "SetField",
            ReturnType = typeof(void),
            Parameters = new ParameterSig[]
            {
                new ParameterSig
                {
                    Name = "field"
                }
            },
        };

        void writeSetFieldMethod(MethodSig method, CodeBuilder codeBuilder)
        {

        }


        var cmw = new CustomMethodWriter(writeSetFieldMethod);
        implDeets.MemberWriters.Add((setFieldMethod, cmw));


    }

    public override void Write(PropertySig property, CodeBuilder codeBuilder)
    {
        // We know we created a backing field
        var names = GetBackingFieldNames(property);

        codeBuilder
          .AppendValue(property.Visibility, "lc")
          .AppendIf(property.Instic == Instic.Instance, " ", " static ")
          .AppendKeywords(property.Keywords)
          .AppendLine(property.Name)
          .BracketBlock(propBlock =>
          {
              // Getter?
              if (property.GetMethod is not null)
              {
                  propBlock.CodeLine($"get => this.{names.Name};");
              }
              // Setter?
              var setMethod = property.SetMethod;
              if (setMethod is not null)
              {
                  if (setMethod.Keywords.HasFlag(Keywords.Init))
                  {
                    propBlock.AppendLine("init");
                      
                  }
                  else
                  {
                      propBlock.AppendLine("set");
                  }
                  propBlock.BracketBlock(setBlock =>
                  {

                  })
              }
          }).NewLine();
    }
}
