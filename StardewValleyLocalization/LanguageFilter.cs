namespace StardewValleyLocalization
{
    public class LanguageFilter
    {
        public string Name { get; set; }
        public string[] RegexFilenameFilters { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}