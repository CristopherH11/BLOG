using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

public static class CustomHtmlHelper
{
    public static IHtmlContent Truncate(this IHtmlHelper helper, string text, int id, int maxLength)
    {
        if (text.Length <= maxLength)
        {
            return new HtmlString(text);
        }

        string truncatedText = text.Substring(0, maxLength);
        return new HtmlString($"{truncatedText}... <a href=\"Posts/Details/{id}\">Leer más</a>");
    }
}

