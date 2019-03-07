using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using PropertyChanged;
using StardewValleyLocalization.Parsers;

namespace StardewValleyLocalization
{
    interface IContentValidator
    {
        List<Warning> Validate(string oldContent, string newContent);
    }

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

        public void InitializeContentParts(string content)
        {
            bool validate = false;
            if (ContentParts != null)
            {
                validate = true;
                foreach (var cp in ContentParts)
                {
                    cp.PropertyChanged -= ContentChanged;
                }
            }

            ContentParts = new ReadOnlyCollection<ObservableContent>(_parser.DisassembleToParts(content).Select(
                x =>
                {
                    var observableContent = new ObservableContent {Name = x.Key, Content = x.Value};
                    observableContent.PropertyChanged += ContentChanged;

                    return observableContent;
                }).ToList());
            if (validate)
            {
                ValidateContent();
                ContentChanged(null, null);
            }
        }

        private readonly List<IContentValidator> contentValidators = new List<IContentValidator>
        {
            new LeadingAndTrailingSpaceContentValidator(),
            new FormatSpecifierValidator(),
            new StringEndCharacterMatch(),
            new BracketsContentValidator(),
            new CapitalizedContentValidator(),
            new EllipsisValidator()
        };

        private void ValidateContent()
        {
            Warnings.RaiseListChangedEvents = false;

            Warnings.Clear();

            if (string.IsNullOrEmpty(Original) || string.IsNullOrEmpty(Result))
            {
                Warnings.Add(new Warning ("No content", WarningLevel.Error));
            }
            else
            {
                contentValidators.ForEach(c => {  c.Validate(Original, Result).ForEach(Warnings.Add); });
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