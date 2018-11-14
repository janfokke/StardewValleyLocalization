using System;
using System.Collections.Generic;
using XnbConvert.Readers;

namespace XnbConvert
{
    public static class XnbReaderFactory
    {
        private static readonly Dictionary<Type, WeakReference<XnbTypeReader>> ReaderCache = new Dictionary<Type, WeakReference<XnbTypeReader>>();
        private static readonly object ReaderCacheLock = new object();

        public static XnbTypeReader CreateReaderFromType<TXnbReader>() => CreateReaderFromType(typeof(TXnbReader));
       
        public static XnbTypeReader CreateReaderFromType(Type type)
        {
            
            if(type.IsAssignableFrom(typeof(XnbTypeReader)))
            {
                throw new XnbException("Not a valid XnbReader");
            }

            lock (ReaderCacheLock)
            {
                if (ReaderCache.TryGetValue(type, out WeakReference<XnbTypeReader> result))
                {
                    if (result.TryGetTarget(out XnbTypeReader target))
                    {
                        return target;
                    }
                    //Renew
                    XnbTypeReader instance = CreateInstance(type);
                    ReaderCache[type].SetTarget(instance);
                    return instance;
                }
                else
                {
                    XnbTypeReader instance = CreateInstance(type);
                    ReaderCache[type] = new WeakReference<XnbTypeReader>(instance);
                    return instance;
                }
            }
        }

        private static XnbTypeReader CreateInstance(Type type)
        {
            return Activator.CreateInstance(type) as XnbTypeReader;
        }
    } 
}