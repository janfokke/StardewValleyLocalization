using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewValleyLocalization
{
    class LanguageFilter
    {
        public string Name { get; set; }
        public string[] RegexFilenameFilters { get; set; }
    }

    class Settings
    {
        private static Settings _instance;

        public static Settings Instance {
            get
            {
                if (_instance == null)
                {
                    _instance = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("Settings.json"));
                }
                return _instance;
            }
        }

        public string ContentRoot { get; set; }
        public string[] DirectoriesThatContainLanguageFiles { get; set; }
        public LanguageFilter[] LanguageFilters { get; set; }
    }
}
