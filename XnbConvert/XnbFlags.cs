using System;

namespace XnbConvert
{
    [Flags]
    public enum XnbFlags : byte
    {
        /// <summary>
        /// content is for HiDef profile
        /// </summary>
        Hidef = 0x01,

        /// <summary>
        /// asset data is compressed
        /// </summary>
        ContentCompressedLzx = 0x80
    }
}