using System.IO;
using System.Text;

namespace XnbConvert
{
    public class XnbStreamReader : BinaryReader
    {
        public XnbStreamReader(Stream input) : base(input)
        {

        }

        public XnbStreamReader(Stream input, Encoding encoding) : base(input, encoding)
        {
        }

        public XnbStreamReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
        }

        public new int Read7BitEncodedInt()
        {
            return base.Read7BitEncodedInt();
        }
    }

    public class XnbStreamWriter : BinaryWriter
    {
        public XnbStreamWriter(Stream input) : base(input)
        {

        }

        public XnbStreamWriter(Stream input, Encoding encoding) : base(input, encoding)
        {
        }

        public XnbStreamWriter(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
        {
        }

        public new void Write7BitEncodedInt(int value)
        {
            base.Write7BitEncodedInt(value);
        }
    }
}