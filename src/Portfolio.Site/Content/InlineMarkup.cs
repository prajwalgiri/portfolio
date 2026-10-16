using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

namespace Portfolio.Site.Content;

/// <summary>
/// Tiny, safe inline formatter for content strings: <c>**text**</c> becomes bold,
/// <c>==text==</c> becomes bold in the accent colour. Everything else is HTML-encoded.
/// </summary>
public static partial class InlineMarkup
{
    [GeneratedRegex(@"(\*\*(?<bold>.+?)\*\*)|(==(?<accent>.+?)==)")]
    private static partial Regex Token();

    public static MarkupString Render(string text)
    {
        var html = new StringBuilder();
        var position = 0;

        foreach (Match match in Token().Matches(text))
        {
            html.Append(WebUtility.HtmlEncode(text[position..match.Index]));

            if (match.Groups["accent"].Success)
            {
                html.Append("<strong class=\"accent\">")
                    .Append(WebUtility.HtmlEncode(match.Groups["accent"].Value))
                    .Append("</strong>");
            }
            else
            {
                html.Append("<strong>")
                    .Append(WebUtility.HtmlEncode(match.Groups["bold"].Value))
                    .Append("</strong>");
            }

            position = match.Index + match.Length;
        }

        html.Append(WebUtility.HtmlEncode(text[position..]));
        return new MarkupString(html.ToString());
    }
}
