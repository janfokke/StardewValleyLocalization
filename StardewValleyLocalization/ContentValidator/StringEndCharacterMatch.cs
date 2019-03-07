using System.Collections.Generic;
using System.Linq;

namespace StardewValleyLocalization
{
    class StringEndCharacterMatch : IContentValidator
    {
        public List<Warning> Validate(string oldContent, string newContent)
        {
            var warnings = new List<Warning>();

            var originalTrimmed = oldContent.TrimEnd();
            var resultTrimmed = newContent.TrimEnd();
            var trimmedChars = newContent.Remove(0, resultTrimmed.Length);

            var filters = new Dictionary<char, string>
            {
                {'.', "dot"},
                {'!', "exclamation mark"},
                {'?', "question mark"},
                {',', "comma "},
                {';', "semicolon "},
                {':', "Colon "}
            };

            var originalLastChar = originalTrimmed.Last();
            if (filters.TryGetValue(originalLastChar, out var name) &&
                originalLastChar != resultTrimmed.Last())
                warnings.Add(new Warning
                (
                 $"Result does not end with a {name}!",
                    WarningLevel.Info,
                 resultTrimmed+originalLastChar+trimmedChars

                ));
            return warnings;
        }
    }
}