using System;

namespace XnbConvert.Exceptions
{
    public class InvalidXnbTypeReaderNameException : Exception
    {
        public string XnbTypeReaderName { get; }

        public InvalidXnbTypeReaderNameException(string xnbTypeReaderName)
        {
            XnbTypeReaderName = xnbTypeReaderName;
        }
    }
}