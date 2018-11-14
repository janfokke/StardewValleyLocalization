using System.Collections.Generic;

namespace XnbConvert
{
    public class XnbFile<T>
    {
        public XnbTargetOs XnbTargetOs { get; set; }
        public XnbFormatVersion FormatVersion { get; set; }
        public XnbFlags Flags { get; set; }
        private IEnumerable<XnbTypeReader> Readers { get; set; }
        public T Content { get; set; }
        public int Size { get; set; }
    }
}