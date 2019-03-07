using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace StardewValleyLocalization
{
    class EllipsisValidator : IContentValidator
    {
        public List<Warning> Validate(string oldContent, string newContent)
        {
            var warnings = new List<Warning>();

            var regex = new Regex(@"\.+\.");
            var matches = regex.Matches(newContent);
            foreach (Match match in matches)
            {
                if (match.Value == "...")
                    continue;

                StringBuilder fixBuilder = new StringBuilder(newContent);
                fixBuilder.Remove(match.Index, match.Length).Insert(match.Index, "...");
                warnings.Add(
                        new Warning(
                            "Invalid Ellipsis",
                            WarningLevel.Info,
                            fixBuilder.ToString()
                        ))
                    ;
            }

            return warnings;
        }
    }
}