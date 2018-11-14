using System;
using System.Collections.Generic;

namespace XnbConvert.Readers
{
    public class StringTypeReader : XnbTypeReader
    {
        public override int Version => 0;
        public override string Name => "Microsoft.Xna.Framework.Content.StringReader";

        public override object Read(XnbReaderManager xnbReaderManager, XnbStreamReader xnbStreamReader)
        {
            return xnbStreamReader.ReadString();
        }

        public override void Write(XnbReaderManager xnbReaderManager, XnbStreamWriter xnbStreamWriter, object value)
        {
            if (value is string text)
            {
                xnbStreamWriter.Write(text);
            }
        }
    }
}