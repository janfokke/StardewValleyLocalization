using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XnbConvert
{
    public class XnbSerializer
    {
        private static readonly Lazy<XnbSerializer> _lazy = new Lazy<XnbSerializer>(() => new XnbSerializer());
        public static XnbSerializer Instance => _lazy.Value;

        public XnbSerializer()
        {
            
        }

        public byte[] Serialize<T>(T value, 
            XnbTargetOs xnbTargetOs = XnbTargetOs.MicrosoftWindow,
            byte formatVersion = 5,
            XnbFlags flags = 0
        )
        {
            var xnbStreamWriter = new XnbStreamWriter(new MemoryStream());
            
            // write XNB default header
            xnbStreamWriter.Write('X');
            xnbStreamWriter.Write('N');
            xnbStreamWriter.Write('B');
            
            // write XNB target OS
            xnbStreamWriter.Write((byte) xnbTargetOs);
            
            // write XNB format version
            xnbStreamWriter.Write(formatVersion);
            
            // write XNB flags
            // remove compression flag
            flags = flags & ~XnbFlags.ContentCompressedLzx;
            xnbStreamWriter.Write((byte)flags);

            //tmp file size
            long sizeIndex = xnbStreamWriter.BaseStream.Position;
            xnbStreamWriter.Write(0);

            //TODO: encoding here

            Type readerType = XnbTypeReaderTypeResolver.ResolveFromTargetType<T>();
            XnbTypeReader reader = XnbReaderFactory.CreateReaderFromType(readerType);
            var xnbReaderManager = new XnbReaderManager(reader);
            var tmpStream = new XnbStreamWriter(new MemoryStream());
            reader.Write(xnbReaderManager ,tmpStream, value);
            List<XnbTypeReader> xnbTypeReaders = xnbReaderManager.XnbTypeReaders;
            
            // write reader count
            xnbStreamWriter.Write7BitEncodedInt(xnbTypeReaders.Count);
            
            // write readers and version
            foreach (XnbTypeReader xnbTypeReader in xnbTypeReaders)
            {
                xnbStreamWriter.Write(xnbTypeReader.Name);
                xnbStreamWriter.Write(xnbTypeReader.Version);
            }
            
            // no shared resources
            xnbStreamWriter.Write7BitEncodedInt(0);
            
            // write initial reader index
            xnbStreamWriter.Write7BitEncodedInt(1);

            tmpStream.BaseStream.Position = 0;
            tmpStream.BaseStream.CopyTo(xnbStreamWriter.BaseStream);

            xnbStreamWriter.BaseStream.Position = sizeIndex;
            xnbStreamWriter.Write((int)xnbStreamWriter.BaseStream.Length);
            
            using (var ms = new MemoryStream())
            {
                xnbStreamWriter.BaseStream.Position = 0;
                xnbStreamWriter.BaseStream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}