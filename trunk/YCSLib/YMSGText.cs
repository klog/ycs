
/*
 * This source code is provided "as is" and without warranties as
 * to fitness for a particular purpose or merchantability. You may
 * use, distribute and modify this code under the terms of the
 * Microsoft Public License (Ms-PL) and you must retain all copyright,
 * patent, trademark, and attribution notices that are present in
 * the software.
 * 
 * You should have received a copy of the Microsoft Public License with
 * this file. If not, please write to: wickedcoder@hotmail.com,
 * or visit : http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
 * Copyright (C) 2010 Wickedcoder - All Rights Reserved.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YCSLib
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
