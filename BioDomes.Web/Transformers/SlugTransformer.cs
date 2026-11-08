using BioDomes.Domains;

namespace BioDomes.Web.Transformers;

public class SlugTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        if (value == null)
            return null;

        var stringValue = value.ToString() ?? string.Empty;
        return stringValue.ToKebabCase();
    }
}