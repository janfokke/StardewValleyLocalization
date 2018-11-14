using System;
using System.Collections.Generic;
using NUnit.Framework;
using XnbConvert.Readers;

namespace XnbConvert.Test
{
    [TestFixture]
    public class XnbTypeReaderResolverTests
    {
        #region ReaderNames
        private const string DictionaryStringStringReaderName = @"Microsoft.Xna.Framework.Content.DictionaryReader`2[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]";
        private const string StringReaderName = @"Microsoft.Xna.Framework.Content.StringReader";
        #endregion

        [Test]
        [TestCase(DictionaryStringStringReaderName, typeof(DictionaryTypeReader<string,string>))]
        [TestCase(StringReaderName, typeof(StringTypeReader))]
        public void ResolveTypeReaderFromNameTest(string readerName, Type expectedType)
        {
            Type readerType = XnbTypeReaderTypeResolver.ResolveFromName(readerName);
            Assert.AreEqual(expectedType, readerType);
        }
        
        [Test]
        [TestCase(typeof(Dictionary<string, string>), typeof(DictionaryTypeReader<string,string>))]
        [TestCase(typeof(string), typeof(StringTypeReader))]
        public void ResolveTypeReaderFromTargetTypeTest(Type targetType, Type expectedReaderType)
        {
            Type readerType = XnbTypeReaderTypeResolver.ResolveFromTargetType(targetType);
            Assert.AreEqual(expectedReaderType, readerType);
        }
    }
}
