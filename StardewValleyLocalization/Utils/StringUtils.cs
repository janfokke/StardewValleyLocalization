using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StardewValleyLocalization.Utils
{
    static class StringUtils
    {
        public static string ReplaceRegexMatch(this string newContent, Match oldMatch, Match newMatch)
        {
            var fixStringBuilder = new StringBuilder(newContent);
            
            fixStringBuilder
                .Remove(oldMatch.Index, oldMatch.Length)
                .Insert(oldMatch.Index, newMatch.Value);
            return fixStringBuilder.ToString();
        }
    }
}
