using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValleyLocalization.Parsers
{
    internal class SplitStringParser : IParser
    {
        public string Separator { get; set; }
        public List<KeyValuePair<int, string>> ContentIndexes { get; set; }

        public List<KeyValuePair<string, string>> DisassembleToParts(string original)
        {
            var split = original.Split(new[] {Separator}, StringSplitOptions.None);

            var parts = new List<KeyValuePair<string, string>>();
            foreach (var index in ContentIndexes)
                // Non-english content sometimes has extra info, this is to avoid index exception
                if (index.Key < split.Length)
                    parts.Add(new KeyValuePair<string, string>(index.Value, split[index.Key]));

            return parts;
        }

        public string ReassembleParts(List<string> parts, string original)
        {
            if (parts.Any(p => p.Contains(Separator)))
                throw new Exception($"'{Separator}' character cannot be used here");

            var split = original.Split(new[] {Separator}, StringSplitOptions.None);
            var index = 0;
            foreach (var contentIndex in ContentIndexes) split[contentIndex.Key] = parts[index++];
            return string.Join(Separator, split);
        }
    }
}