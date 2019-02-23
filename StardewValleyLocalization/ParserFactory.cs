using System;
using System.Collections.Generic;
using System.Linq;
using StardewValleyLocalization.Parsers;

namespace StardewValleyLocalization
{
    internal class ParserFactory
    {
        private static readonly Lazy<ParserFactory> _lazy = new Lazy<ParserFactory>(() => new ParserFactory());

        private readonly List<KeyValuePair<string, IParser>> _parsers;

        private ParserFactory()
        {
            _parsers = Settings.Instance.ContentParsers;
        }

        public static ParserFactory Instance => _lazy.Value;

        public IParser FindParser(string relativePath)
        {
            var parser = _parsers.FirstOrDefault(keyValuePair => relativePath.Contains(keyValuePair.Key));
            return parser.Value ?? FullStringParser.Instance;
        }
    }
}