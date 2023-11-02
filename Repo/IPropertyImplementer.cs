using Implementor.SourceGen.Writers;

namespace Implementor.SourceGen;

public interface IPropertyImplementer
{
    IPropertySigWriter GetPropertyWriter();


}
