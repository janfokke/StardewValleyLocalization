using System;
using System.Collections.Generic;
using System.IO;

namespace XnbConvert.Terminal
{
    class Program
    {
        static void Main(string[] args)
        {
            var xnbDeserializer = new XnbDeserializer();
            byte[] xnbData = File.ReadAllBytes("/home/janfokke/.steam/steam/steamapps/common/Stardew Valley/Content/Strings/schedules/Abigail.xnb");
            XnbFile<Dictionary<string, string>> test = xnbDeserializer.Deserialize<Dictionary<string, string>>(xnbData);
            
            var xnbSerializer = new XnbSerializer();
            byte[] d = xnbSerializer.Serialize(test.Content);
            XnbFile<Dictionary<string, string>> test2 = xnbDeserializer.Deserialize<Dictionary<string, string>>(d);
        }
    }
}