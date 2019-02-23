using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using PropertyChanged;
using StardewValleyLocalization.Parsers;

namespace StardewValleyLocalization
{
    [AddINotifyPropertyChangedInterface]
    internal class Content
    {
        private readonly XnbFile _file;

        /// <summary>
        ///     Parser, used for dividing the content into easy to use parts
        /// </summary>
        private readonly IParser _parser;

        private string prevContent;

        public Content(XnbFile file, IParser parser, object index, string content, string contentPath)
        {
            _file = file;
            _parser = parser;
            Index = index;
            Original = prevContent = content;
            ContentPath = contentPath;
            Result = content;
            InitializeContentParts(content);
        }

        /// <summary>
        ///     Content index
        /// </summary>
        public object Index { get; }

        /// <summary>
        ///     Original content
        /// </summary>
        public string Original { get; private set; }

        /// <summary>
        ///     The relative path of the file without language and .xnb prefix + :FileIndex
        /// </summary>
        public string ContentPath { get; }


        public BindingList<Warning> Warnings { get; set; } = new BindingList<Warning>();

        /// <summary>
        ///     Original divided into easy to use parts
        /// </summary>
        public ReadOnlyCollection<ObservableContent> ContentParts { get; set; }

        /// <summary>
        ///     Reassembled <see cref="ContentParts" />
        /// </summary>
        public string Result { get; private set; }

        private void InitializeContentParts(string content)
        {
            ContentParts = new ReadOnlyCollection<ObservableContent>(_parser.DisassembleToParts(content).Select(
                x =>
                {
                    var observableContent = new ObservableContent {Name = x.Key, Content = x.Value};
                    observableContent.PropertyChanged += ContentChanged;

                    return observableContent;
                }).ToList());
        }

        private void ValidateContent()
        {
            Warnings.RaiseListChangedEvents = false;

            Warnings.Clear();

            if (string.IsNullOrEmpty(Original) || string.IsNullOrEmpty(Result))
            {
                Warnings.Add(new Warning {Message = "No content", WarningLevel = WarningLevel.Error});
            }
            else
            {
                var specialEncodings = new[]
                {
                    "$a",
                    "{",
                    "}",
                    "{0}",
                    "{1}",
                    "{2}",
                    "{3}",
                    "{4}",
                    "{5}",
                    "{6}",
                    "{7}",
                    "{8}",
                    "{9}",
                    "/",
                    "$content",
                    "$neutral",
                    "\\n",
                    "$h",
                    "$l",
                    "$9",
                    "$a",
                    "$b",
                    "$c",
                    "$d",
                    "$y",
                    "$q",
                    "$r",
                    "$p",
                    "$k",
                    "$e",
                    "$s",
                    "$u",
                    "#",
                    "¦",
                    "$",
                    "%adj",
                    "%noun",
                    "%place",
                    "%name",
                    "%firstnameletter",
                    "%band",
                    "%book",
                    "%pet",
                    "%favorite",
                    "%farm",
                    "%time",
                    "%fork",
                    "%kid1",
                    "%kid2",
                    "*",
                    "@",
                    "^",
                    "%",
                    "%spouse",
                    "%rival"
                };
                foreach (var specialEncoding in specialEncodings)
                {
                    var oldStringCount = Original.Split(new[] {specialEncoding}, StringSplitOptions.None).Length - 1;
                    var newStringCount = Result.Split(new[] {specialEncoding}, StringSplitOptions.None).Length - 1;
                    if (oldStringCount == 0 || oldStringCount == newStringCount)
                        continue;

                    Warnings.Add(new Warning
                    {
                        Message =
                            $"[{specialEncoding}] occurs {oldStringCount} time(s) in old content and {newStringCount} time(s) in new content!",
                        WarningLevel = WarningLevel.Error
                    });
                }

                if (Original.EndsWith(" ") && !Result.EndsWith(" "))
                    Warnings.Add(new Warning
                    {
                        Message = "Trailing space missing!",
                        WarningLevel = WarningLevel.Warning
                    });

                if (!Original.EndsWith(" ") && Result.EndsWith(" "))
                    Warnings.Add(new Warning
                    {
                        Message = "Unnecessary trailing space!",
                        WarningLevel = WarningLevel.Warning
                    });

                if (Original.StartsWith(" ") && !Result.StartsWith(" "))
                    Warnings.Add(new Warning
                    {
                        Message = "Leading space missing!",
                        WarningLevel = WarningLevel.Warning
                    });

                if (!Original.StartsWith(" ") && Result.StartsWith(" "))
                    Warnings.Add(new Warning
                    {
                        Message = "Unnecessary Leading space!",
                        WarningLevel = WarningLevel.Warning
                    });

                var originalTrimmed = Original.TrimEnd();
                var resultTrimmed = Result.TrimEnd();

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
                    Warnings.Add(new Warning
                    {
                        Message = $"Result does not end with a {name}!",
                        WarningLevel = WarningLevel.Info
                    });

                var balancedChars = new List<Tuple<char, char, string>>
                {
                    new Tuple<char, char, string>('(', ')', "Parentheses"),
                    new Tuple<char, char, string>('{', '}', "Curly braces"),
                    new Tuple<char, char, string>('[', ']', "Square brackets")
                };

                foreach (var encoding in balancedChars)
                {
                    var openCount = Result.Split(new[] {encoding.Item1}, StringSplitOptions.None).Length - 1;
                    var closeCount = Result.Split(new[] {encoding.Item2}, StringSplitOptions.None).Length - 1;

                    if (openCount == 0 && closeCount == 0
                        || closeCount == openCount)
                        continue;

                    Warnings.Add(new Warning
                    {
                        Message = $"opening and closing {encoding.Item3} are not balanced!",
                        WarningLevel = WarningLevel.Info
                    });
                }


                var firstLetterOriginal = Original.FirstOrDefault(char.IsLetter);
                var firstLetterResult = Result.FirstOrDefault(char.IsLetter);
                if (firstLetterOriginal != '\0' && firstLetterResult != '\0')
                {
                    if (char.IsUpper(firstLetterOriginal) && !char.IsUpper(firstLetterResult))
                        Warnings.Add(new Warning
                        {
                            Message = "First letter is not capitalized!",
                            WarningLevel = WarningLevel.Info
                        });

                    if (!char.IsUpper(firstLetterOriginal) && char.IsUpper(firstLetterResult))
                        Warnings.Add(new Warning
                        {
                            Message = "First letter is capitalized!",
                            WarningLevel = WarningLevel.Info
                        });
                }
            }

            Warnings.RaiseListChangedEvents = true;
            Warnings.ResetBindings();
        }

        private void ContentChanged(object sender, PropertyChangedEventArgs e)
        {
            _file.Changed = true;
            try
            {
                Result = _parser.ReassembleParts(ContentParts.Select(x => x.Content).ToList(), Original);
                prevContent = Result;
                ValidateContent();
            }
            catch (Exception ex)
            {
                InitializeContentParts(prevContent);
                Warnings.Add(new Warning {WarningLevel = WarningLevel.Error, Message = ex.Message});
            }
        }

        /// <summary>
        ///     Overrides the Original value with Resulting value
        /// </summary>
        public void Save()
        {
            Original = Result;
        }
    }
}