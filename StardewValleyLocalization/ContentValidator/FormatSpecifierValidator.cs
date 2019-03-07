using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using StardewValleyLocalization.Utils;

namespace StardewValleyLocalization
{
    class FormatSpecifierValidator : IContentValidator
    {
        private readonly string[] formatSpecifiers = {
            "$a",
            "{0}",
            "{1}",
            "{2}",
            "{3}",
            "{4}",
            "{5}",
            "{6}",
            "{7}",
            "{8}",
            "{9}",
            "/",
            "$content",
            "$neutral",
            "\\n",
            "$h",
            "$l",
            "$9",
            "$a",
            "$b",
            "$c",
            "$d",
            "$y",
            "$q",
            "$r",
            "$p",
            "$k",
            "$e",
            "$s",
            "$u",
            "#",
            "¦",
            "$",
            "%adj",
            "%noun",
            "%place",
            "%name",
            "%firstnameletter",
            "%band",
            "%book",
            "%pet",
            "%favorite",
            "%farm",
            "%time",
            "%fork",
            "%kid1",
            "%kid2",
            "*",
            "@",
            "^",
            "%",
            "%spouse",
            "%rival"
        };
        
        public List<Warning> Validate(string oldContent, string newContent)
        {
            var warnings = new List<Warning>();

            foreach (var formatSpecifier in formatSpecifiers)
            {
                var regex = new Regex($" *{Regex.Escape(formatSpecifier)} *");
                var oldContentMatches = regex.Matches(oldContent);
                if (oldContentMatches.Count == 0)
                    continue;

                var newContentMatches = regex.Matches(newContent);

                for (int i = 0; i < Math.Min(oldContentMatches.Count, newContentMatches.Count); i++)
                {
                    if (oldContentMatches[i].Value != newContentMatches[i].Value)
                    {
                        var newContentMatch = newContentMatches[i];
                        var oldContentMatch = oldContentMatches[i];
                        string contentFix = newContent.ReplaceRegexMatch(newContentMatch, oldContentMatch);

                        warnings.Add
                        (
                            new Warning
                            (
                                $"Number of leading/trailing spaces of Format specifier:{formatSpecifier} do not match",
                                WarningLevel.Info,
                                contentFix
                            )
                        );
                    }
                }

                if (oldContentMatches.Count == newContentMatches.Count)
                    continue;

                warnings.Add
                (
                    new Warning
                    (
                        $"[{formatSpecifier}] occurs {oldContentMatches.Count} time(s) in old content and {newContentMatches.Count} time(s) in new content!",
                        WarningLevel.Error
                    )
                );
            }

            return warnings;
        }

        
    }
}