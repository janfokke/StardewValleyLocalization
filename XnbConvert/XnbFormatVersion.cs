using System;

namespace XnbConvert
{
    public struct XnbFormatVersion
    {
        /// <summary>
        /// XNB Format Version
        /// </summary>
        public Version Version { get; }

        /// <summary>
        /// FormatIndicator used for Serialization and Deserialization
        /// </summary>
        public byte Indicator { get; }

        /// <summary>
        /// XNB Format Version: XNA Game Studio 3.0
        /// </summary>
        public static XnbFormatVersion XnaGameStudio30 = new XnbFormatVersion(new Version(3,0), 0x3);

        /// <summary>
        /// XNB Format Version: XNA Game Studio 3.1
        /// </summary>
        public static XnbFormatVersion XnaGameStudio31 = new XnbFormatVersion(new Version(3,1), 0x4);

        /// <summary>
        /// XNB Format Version: XNA Game Studio 4.0
        /// </summary>
        public static XnbFormatVersion XnaGameStudio40 = new XnbFormatVersion(new Version(4,0), 0x5);

        public XnbFormatVersion(Version version, byte indicator)
        {
            Version = version;
            Indicator = indicator;
        }
    }
}