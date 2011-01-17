using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YMSGLib
{
    [Flags]
    public enum YmsgStripTagOptions
    {
        StripOldYahooFormatTags = 1,
        StripHtmlFormatTags = 2,
        StripAll = 3
    }
    
    public class YMSGText
    {
        private static string StripOldYahooFormatTags(string source)
        {
            Regex r1 = new Regex(@"\x1b*\[[#?|x?]?[a-f0-9A-F]{0,6}m",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return r1.Replace(source, "");
        }

        private static string StripHtmlFormatTags(string source)
        {
            Regex r1 = new Regex(@"<[/]?(font|fade|bold|alt|italic|b|underline|u|i|gray|[FADE]:\w+)[^>]*?>",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return r1.Replace(source, "");
        }

        public static string StripTags(string text, YmsgStripTagOptions options)
        {
            string retVal = text;

            if ((options & YmsgStripTagOptions.StripOldYahooFormatTags) == YmsgStripTagOptions.StripOldYahooFormatTags)
                retVal = StripOldYahooFormatTags(retVal);
            if ((options & YmsgStripTagOptions.StripHtmlFormatTags) == YmsgStripTagOptions.StripHtmlFormatTags)
                retVal = StripHtmlFormatTags(retVal);

            return retVal;
        }

    }
}
