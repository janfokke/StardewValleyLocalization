using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnbConvert
{
    public abstract class XnbTypeReader
    {
        public abstract int Version { get; }
        
        public abstract object Read(XnbReaderManager xnbReaderManager, XnbStreamReader xnbStreamReader);

        public abstract void Write(XnbReaderManager xnbReaderManager, XnbStreamWriter xnbStreamWriter, object value);

        public abstract string Name { get; }
    }
}
