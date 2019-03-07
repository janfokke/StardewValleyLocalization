using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace StardewValleyLocalization
{
    class CapitalizedContentValidator : IContentValidator
    {
        public List<Warning> Validate(string oldContent, string newContent)
        {
            var warnings = new List<Warning>();

            var regex = new Regex(@"[a-zA-Z]");
            Match oldContentMatch = regex.Match(oldContent);
            Match newContentMatch = regex.Match(newContent);

            if (oldContentMatch.Success && newContentMatch.Success)
            {
                if (char.IsUpper(oldContentMatch.Value.First()) && !char.IsUpper(newContentMatch.Value.First()))
                {
                    var fixBuilder = new StringBuilder(newContent);
                    fixBuilder.Remove(newContentMatch.Index, 1)
                        .Insert(newContentMatch.Index, newContentMatch.Value.ToUpper());

                    warnings.Add(new Warning
                    (
                        "First letter is not capitalized!",
                        WarningLevel.Info,
                        fixBuilder.ToString()
                    ));
                }

                if (!char.IsUpper(oldContentMatch.Value.First()) && char.IsUpper(newContentMatch.Value.First()))
                {
                    var fixBuilder = new StringBuilder(newContent);
                    fixBuilder.Remove(newContentMatch.Index, 1)
                        .Insert(newContentMatch.Index, newContentMatch.Value.ToLower());

                    warnings.Add(new Warning
                    (
                        "First letter is capitalized!",
                        WarningLevel.Info,
                        fixBuilder.ToString()
                    ));
                }

            }
            return warnings;
        }
    }
}