using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace BioDomes.Domains;

public static class StringExtentions
{
    private static readonly Regex CamelCaseBoundary = new(@"(?<=[a-z0-9])(?=[A-Z])", RegexOptions.Compiled);
    private static readonly Regex NonAlphaNum = new(@"[^A-Za-z0-9]+", RegexOptions.Compiled);
    private static readonly Regex CollapseHyphens = new(@"-{2,}", RegexOptions.Compiled);

    public static string ToKebabCase(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;
        
        value = RemoveDiacritics(value);

        var withBoundaries = CamelCaseBoundary.Replace(value, "-");
        var replaced = NonAlphaNum.Replace(withBoundaries, "-");
        var collapsed = CollapseHyphens.Replace(replaced, "-").Trim('-');

        return collapsed.ToLowerInvariant();
    }
    
    private static string RemoveDiacritics(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var c in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(c);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}