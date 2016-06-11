using System;
using System.Web;
using System.Web.Mvc;

public static class HtmlHelpers
{
    /// <summary>
    /// Replaces new lines with BR tags but ensures that the rest of the content is encoded.
    /// </summary>
    /// <param name="helper"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static HtmlString Multiline<T>(this HtmlHelper<T> helper, string value)
    {
        return new HtmlString(helper.Encode(value).Replace(Environment.NewLine, "<br />"));
    }
}