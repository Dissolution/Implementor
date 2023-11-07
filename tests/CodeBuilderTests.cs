using Implementor.Text;

namespace Implementor.Tests;

public class CodeBuilderTests
{
    private static readonly CBA _appendAbc = cb => cb.Append('A').NewLine().Append('B').NewLine().Append('C');
    
    
    [Fact]
    public void IndentAware()
    {
        using var code = new CodeBuilder();
        code.Append($$"""
            public void DoThing()
            {
                {{_appendAbc}}
            }
            """);
        Assert.Equal("""
            public void DoThing()
            {
                A
                B
                C
            }
            """,
            code.ToString());
    }

    [Fact]
    public void NestedIndentAware()
    {
        CBA cba = b => b.Append($$"""
                                  public void DoThing()
                                  {
                                      {{_appendAbc}}
                                  }
                                  """);
        
        using var code = new CodeBuilder();
        code.Append($$"""
                      public void DoThing()
                      {
                          {{cba}}
                      }
                      """);
        Assert.Equal("""
                     public void DoThing()
                     {
                         public void DoThing()
                         {
                             A
                             B
                             C
                         }
                     }
                     """,
            code.ToString());
        return;

       
    }
}