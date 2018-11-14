using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnbConvert.Readers
{
    public class DictionaryTypeReader<TKeyType, TValueType> : XnbTypeReader
    {
        public override int Version { get; }
        public override string Name { get; }

        public DictionaryTypeReader()
        {
            Version = 0;
            Name = $"Microsoft.Xna.Framework.Content.DictionaryReader`2[[{TypeNameResolver.ResolveAssemblyQualifiedName<TKeyType>()}],[{TypeNameResolver.ResolveAssemblyQualifiedName<TKeyType>()}]]";
        }
        
        public override object Read(XnbReaderManager xnbReaderManager, XnbStreamReader xnbStreamReader)
        {
            var dictionary = new Dictionary<TKeyType, TValueType>();
            int count = xnbStreamReader.ReadInt32();
        
            for (var i = 0; i < count; i++)
            {
                //TODO check if reference type and skip reading index
                int keyReaderIndex = xnbStreamReader.Read7BitEncodedInt();
                var key = (TKeyType)xnbReaderManager.XnbReaderFromIndex(keyReaderIndex).Read(xnbReaderManager, xnbStreamReader);

                int valueReaderIndex = xnbStreamReader.Read7BitEncodedInt();
                var value = (TValueType)xnbReaderManager.XnbReaderFromIndex(valueReaderIndex).Read(xnbReaderManager, xnbStreamReader);

                dictionary.Add(key, value);
            }
            return dictionary;
        }

        public override void Write(XnbReaderManager xnbReaderManager, XnbStreamWriter xnbStreamWriter, object value)
        {
            if (value is Dictionary<TKeyType, TValueType> dictionary)
            {
                xnbStreamWriter.Write(dictionary.Count);
                foreach (KeyValuePair<TKeyType,TValueType> keyValuePair in dictionary)
                {
                    (int keyReaderIndex, XnbTypeReader keyReader) = xnbReaderManager.GetOrAddXnbTypeReaderFromTargetType<TKeyType>();
                    xnbStreamWriter.Write7BitEncodedInt(keyReaderIndex);
                    keyReader.Write(xnbReaderManager,xnbStreamWriter,keyValuePair.Key);
                    
                    (int valueReaderIndex, XnbTypeReader valueReader) = xnbReaderManager.GetOrAddXnbTypeReaderFromTargetType<TValueType>();
                    xnbStreamWriter.Write7BitEncodedInt(valueReaderIndex);
                    valueReader.Write(xnbReaderManager,xnbStreamWriter,keyValuePair.Value);
                }
            }
            else
            {
                throw new ArgumentException($"Expected {typeof(Dictionary<TKeyType, TValueType>).Name}, got {value.GetType().Name}");
            }
        }
    }
}
