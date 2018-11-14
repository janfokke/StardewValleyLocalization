using System.Collections.Generic;

namespace StardewValleyLocalization
{
    class XnbFile
    {
        public bool Changed { get; set; }
        public Dictionary<string,string> OriginalContent { get; set; }
        public Dictionary<string,string> ChangeableContent { get; set; }
    }
}
