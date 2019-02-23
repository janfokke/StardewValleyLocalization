using System;
using System.Collections.Generic;

namespace StardewValleyLocalization.Parsers
{
    internal class FullStringParser : IParser
    {
        private static readonly Lazy<IParser> _lazy = new Lazy<IParser>(() => new FullStringParser());

        private FullStringParser()
        {
        }

        public static IParser Instance => _lazy.Value;

        public List<KeyValuePair<string, string>> DisassembleToParts(string original)
        {
            return new List<KeyValuePair<string, string>> {new KeyValuePair<string, string>("Content", original)};
        }

        public string ReassembleParts(List<string> parts, string original)
        {
            return parts[0];
        }
    }
}