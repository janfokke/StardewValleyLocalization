using System;
using System.Windows.Input;

namespace StardewValleyLocalization
{
    internal class Warning
    {
        public Warning()
        {
            
        }

        public Warning(string message, WarningLevel warningLevel)
        {
            Message = message;
            WarningLevel = warningLevel;
        }

        public Warning(string message, WarningLevel warningLevel, string fix) : this(message,warningLevel)
        {
            HasFix = true;
            Fix = fix;
        }

        public WarningLevel WarningLevel { get; set; }
        public string Message { get; set; }

        public bool HasFix { get; } 
        public string Fix { get; }
    }
}