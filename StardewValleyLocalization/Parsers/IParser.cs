using System.Collections.Generic;

namespace StardewValleyLocalization.Parsers
{
    internal interface IParser
    {
        List<KeyValuePair<string, string>> DisassembleToParts(string original);
        string ReassembleParts(List<string> parts, string original);
    }
}