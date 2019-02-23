using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValleyLocalization.Parsers
{
    internal class EventStringParser : IParser
    {
        public List<KeyValuePair<string, string>> DisassembleToParts(string original)
        {
            var parts = new List<KeyValuePair<string, string>>();
            var split = original.Split('/');
            foreach (var command in split)
            {
                if (command.StartsWith("message"))
                {
                    var commandSplit = command.Split(' ');
                    var part = string.Join(" ", commandSplit.Skip(1).ToList());
                    part = part.Substring(1, part.Length - 2);
                    parts.Add(new KeyValuePair<string, string>("message string", part));
                }

                if (command.StartsWith("speak"))
                {
                    var commandSplit = command.Split(' ');
                    var part = string.Join(" ", commandSplit.Skip(2).ToList());


                    if (part.StartsWith("\"") && part.EndsWith("\"")) part = part.Substring(1, part.Length - 2);

                    parts.Add(new KeyValuePair<string, string>("speak string", part));
                }

                if (command.StartsWith("end dialogue"))
                {
                    var commandSplit = command.Split(' ');
                    var part = string.Join(" ", commandSplit.Skip(3).ToList());


                    if (part.StartsWith("\"") && part.EndsWith("\"")) part = part.Substring(1, part.Length - 2);

                    parts.Add(new KeyValuePair<string, string>("end dialogue string", part));
                }
            }

            return parts;
        }

        public string ReassembleParts(List<string> parts, string original)
        {
            if (parts.Any(p => p.Contains('/')))
                throw new Exception("'/' character cannot be used in event strings");

            var split = original.Split('/');
            var replaceIndex = 0;
            for (var index = 0; index < split.Length; index++)
            {
                var command = split[index];
                if (command.StartsWith("message")) split[index] = $"message \"{parts[replaceIndex++]}\"";

                if (command.StartsWith("speak"))
                {
                    var commandSplit = command.Split(' ');
                    var beginParts = string.Join(" ", commandSplit.Take(2).ToList());
                    var endPart = string.Join(" ", commandSplit.Skip(2).ToList());

                    if (endPart.StartsWith("\"") && endPart.EndsWith("\""))
                        split[index] = string.Join(" ", beginParts, $"\"{parts[replaceIndex++]}\"");
                    else
                        split[index] = string.Join(" ", beginParts, parts[replaceIndex++]);
                }

                if (command.StartsWith("end dialogue"))
                {
                    var commandSplit = command.Split(' ');
                    var beginParts = string.Join(" ", commandSplit.Take(3).ToList());
                    var endPart = string.Join(" ", commandSplit.Skip(3).ToList());
                    if (endPart.StartsWith("\"") && endPart.EndsWith("\""))
                        split[index] = string.Join(" ", beginParts, $"\"{parts[replaceIndex++]}\"");
                    else
                        split[index] = string.Join(" ", beginParts, parts[replaceIndex++]);
                }
            }

            return string.Join("/", split);
        }
    }
}