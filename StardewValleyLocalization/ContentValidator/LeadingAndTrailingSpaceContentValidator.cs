using System.Collections.Generic;
using System.Text;

namespace StardewValleyLocalization
{
    class LeadingAndTrailingSpaceContentValidator : IContentValidator
    {
        public List<Warning> Validate(string oldContent, string newContent)
        {
            var warnings = new List<Warning>();

            if (oldContent.EndsWith(" ") && !newContent.EndsWith(" "))
                warnings.Add(new Warning
                (
                    "Trailing space missing!",
                    WarningLevel.Warning,
                    newContent + " "
                ));

            if (!oldContent.EndsWith(" ") && newContent.EndsWith(" "))
                warnings.Add(new Warning
                (
                    "Unnecessary trailing space!",
                    WarningLevel.Warning,
                    newContent.TrimEnd(' ')
                ));

            if (oldContent.StartsWith(" ") && !newContent.StartsWith(" "))
                warnings.Add(new Warning
                (
                    "Leading space missing!",
                    WarningLevel.Warning,
                    " "+newContent
                ));

            if (!oldContent.StartsWith(" ") && newContent.StartsWith(" "))
                warnings.Add(new Warning
                (
                    "Unnecessary Leading space!",
                    WarningLevel.Warning,
                    newContent.TrimStart(' ')
                ));

            return warnings;
        }
    }
}