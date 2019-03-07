using System;
using System.Collections.Generic;

namespace StardewValleyLocalization
{
    class BracketsContentValidator : IContentValidator
    {
        public List<Warning> Validate(string oldContent, string newContent)
        {
            var warnings = new List<Warning>();
            var balancedChars = new List<Tuple<char, char, string>>
            {
                new Tuple<char, char, string>('(', ')', "Parentheses"),
                new Tuple<char, char, string>('{', '}', "Curly braces"),
                new Tuple<char, char, string>('[', ']', "Square brackets")
            };

            foreach (var encoding in balancedChars)
            {
                var openCount = newContent.Split(new[] { encoding.Item1 }, StringSplitOptions.None).Length - 1;
                var closeCount = newContent.Split(new[] { encoding.Item2 }, StringSplitOptions.None).Length - 1;

                if (openCount == 0 && closeCount == 0
                    || closeCount == openCount)
                    continue;

                warnings.Add(new Warning
                {
                    Message = $"opening and closing {encoding.Item3} are not balanced!",
                    WarningLevel = WarningLevel.Info
                });
            }
            return warnings;
        }
    }
}