using System;
using System.Collections.Generic;
using System.Linq;
using XnbConvert.Readers;

namespace XnbConvert
{
    public class XnbReaderManager
    {
        private readonly List<XnbTypeReader> _xnbTypeReaders = new List<XnbTypeReader>();
        private bool _resolvedReaders = false;

        public List<XnbTypeReader> XnbTypeReaders => _xnbTypeReaders.ToList();

        public XnbReaderManager()
        {
        }

        public XnbReaderManager(XnbTypeReader initialReader)
        {
            _xnbTypeReaders.Add(initialReader);
        }
        
        public void ReadReaders(XnbStreamReader streamReader)
        {
            lock (_xnbTypeReaders)
            {
                if(_resolvedReaders)
                    throw new XnbException("Already resolved readers");
                _resolvedReaders = true;
                
                int readerCount = streamReader.Read7BitEncodedInt();
                for (var i = 0; i < readerCount; i++)
                {
                    string readerName = streamReader.ReadString();
                    int version = streamReader.ReadInt32();
                    Type readerType = XnbTypeReaderTypeResolver.ResolveFromName(readerName);
                    XnbTypeReader reader = XnbReaderFactory.CreateReaderFromType(readerType);
                    _xnbTypeReaders.Add(reader);
                }
            }
        }


        public (int index, XnbTypeReader reader) GetOrAddXnbTypeReaderFromTargetType<TTargetType>()
        {
            return GetOrAddXnbTypeReaderFromTargetType(typeof(TTargetType));
        }
        
        public (int index, XnbTypeReader reader) GetOrAddXnbTypeReaderFromTargetType(Type targetType)
        {
            Type readerType = XnbTypeReaderTypeResolver.ResolveFromTargetType(targetType);
            return XnbTypeReaderFromReaderType(readerType);
        }

        public (int index, XnbTypeReader reader) XnbTypeReaderFromReaderType<TReader>() where TReader : XnbTypeReader
        {
            return XnbTypeReaderFromReaderType(typeof(TReader));
        }
        
        public (int index, XnbTypeReader reader) XnbTypeReaderFromReaderType(Type readerType)
        {
            lock (_xnbTypeReaders)
            {
                int index = _xnbTypeReaders.FindIndex(x => x.GetType().IsAssignableFrom(readerType));
                if (index == -1)
                {
                    XnbTypeReader readerInstance = XnbReaderFactory.CreateReaderFromType(readerType);
                    _xnbTypeReaders.Add(readerInstance);
                    index = _xnbTypeReaders.Count-1;
                }
                XnbTypeReader reader = _xnbTypeReaders[index];
                return (index+1, reader);
            } 
        }

        public XnbTypeReader XnbReaderFromIndex(int index)
        {
            return index == 0 ? null : _xnbTypeReaders[index-1];
        }
    }
}
