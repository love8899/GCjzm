using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;


namespace Wfm.Core
{
    class JsonHelper
    {
        public static string Serialize(Object obj, Type type)
        {
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
            serializer.WriteObject(stream, obj);
            stream.Position = 0;
            StreamReader reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }


        public static object Deserialize(string objectData, Type type)
        {
            object result;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);

            using (MemoryStream ms = new MemoryStream(System.Text.ASCIIEncoding.UTF8.GetBytes(objectData)))
            {
                result = serializer.ReadObject(ms);
            }

            return result;
        }
    }
}
