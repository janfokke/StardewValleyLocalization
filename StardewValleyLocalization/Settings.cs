using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using StardewValleyLocalization.Parsers;

namespace StardewValleyLocalization
{
    internal class Settings
    {
        private static Settings _instance;

        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("Settings.json"),
                        new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Auto});
                return _instance;
            }
        }

        public string ContentRoot { get; set; }
        public string[] DirectoriesThatContainLanguageFiles { get; set; }
        public LanguageFilter[] LanguageFilters { get; set; }
        public List<KeyValuePair<string, IParser>> ContentParsers { get; set; }
        public List<string> ExcludedFiles { get; set; }
        public List<string> IgnoredContent { get; set; }
    }
}