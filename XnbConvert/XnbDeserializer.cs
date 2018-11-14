using System;
using System.Collections.Generic;
using System.IO;
using XnbConvert.Compression;

namespace XnbConvert
{
    public class XnbDeserializer
    {
        private const int XnbCompressedPrologueSize = 14;

        public XnbFile<T> Deserialize<T>(byte[] data)
        {
            Stream stream = new MemoryStream(data);
            return Deserialize<T>(stream);
        }

        public XnbFile<T> Deserialize<T>(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var xnbResult = new XnbFile<T>();

            var xnbStreamReader = new XnbStreamReader(stream);

            // check if file starts with XNB
            ValidatePrefix(xnbStreamReader);

            // read the XNB target OS
            xnbResult.XnbTargetOs = ReadTargetOs(xnbStreamReader);

            // read the XNB format version
            xnbResult.FormatVersion = ReadFormatVersion(xnbStreamReader);

            // read the XNB flag
            var flags = xnbResult.Flags = ReadFlags(xnbStreamReader);

            // read the file size
            var size = xnbResult.Size = (int)xnbStreamReader.ReadUInt32();

            // check if encoded. If so, decode stream
            xnbStreamReader = DecodeXnbStreamReader(xnbStreamReader, flags, size);

            var xnbReaderManager = new XnbReaderManager();
            xnbReaderManager.ReadReaders(xnbStreamReader);
            
            // ensure there are no shared resources
            var sharedResourcesCount = xnbStreamReader.Read7BitEncodedInt();
            if (sharedResourcesCount != 0)
                throw new NotSupportedException("Shared resources are not supported");
            
            int readerIndex = xnbStreamReader.Read7BitEncodedInt();
            var reader = xnbReaderManager.XnbReaderFromIndex(readerIndex);
            if (reader == null)
            {
                xnbResult.Content = default(T);
            }
            else if (reader.Read(xnbReaderManager, xnbStreamReader) is T content)
            {
                xnbResult.Content = content;
            }
            else
                throw new XnbException("Invalid type");

            return xnbResult;
        }

        private XnbStreamReader DecodeXnbStreamReader(XnbStreamReader endcodedXnbStreamReader, XnbFlags flags, int size)
        {
            // check if file size is equal to stream size
            if (endcodedXnbStreamReader.BaseStream.Length != size)
                throw new XnbException("XNB file has been truncated!");

            // check if stream is compressed
            if (!flags.HasFlag(XnbFlags.ContentCompressedLzx))
                return endcodedXnbStreamReader;

            var decompressedSize = (int)endcodedXnbStreamReader.ReadUInt32();
            var compressedSize = size - XnbCompressedPrologueSize;
            var decompressedStream = new LzxDecoderStream(endcodedXnbStreamReader.BaseStream, decompressedSize, compressedSize);
            return new XnbStreamReader(decompressedStream);
        }

        private XnbFormatVersion ReadFormatVersion(XnbStreamReader xnbStreamReader)
        {
            var formatIndicator = xnbStreamReader.ReadByte();
            switch (formatIndicator)
            {
                case 0x3:
                    return XnbFormatVersion.XnaGameStudio30;
                case 0x4:
                    return XnbFormatVersion.XnaGameStudio31;
                case 0x5:
                    return XnbFormatVersion.XnaGameStudio40;
                default:
                    return new XnbFormatVersion(null, 0);
            }
        }

        private XnbTargetOs ReadTargetOs(XnbStreamReader xnbStreamReader)
        {
            var target = xnbStreamReader.ReadChar();
            switch (target)
            {
                case 'w':
                    return XnbTargetOs.MicrosoftWindow;
                case 'm':
                    return XnbTargetOs.WindowsPhone;
                case 'x':
                    return XnbTargetOs.Xbox360;
                default:
                    return XnbTargetOs.Unknown;
            }
        }

        private XnbFlags ReadFlags(XnbStreamReader xnbStreamReader)
        {
            return (XnbFlags)xnbStreamReader.ReadByte();
        }

        private void ValidatePrefix(XnbStreamReader xnbStreamReader)
        {
            var prefix = new string(xnbStreamReader.ReadChars(3));
            if (prefix != "XNB") throw new XnbException($"Invalid XnbDictionary message, expected XNB, found {prefix}");
        }
    }
}
