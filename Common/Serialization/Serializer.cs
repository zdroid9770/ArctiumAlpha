using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Common.Serialization
{
    public class Serializer
    {
        BinaryFormatter BinarySerializer { get; set; }
        FileStream ObjectStream { get; set; }

        public Serializer()
        {
            BinarySerializer = new BinaryFormatter();
        }

        public void Serialize<T>(T obj)
        {
            ObjectStream = new FileStream(@"Arctium.dat", FileMode.Create);
            BinarySerializer.Serialize(ObjectStream, obj);
            ObjectStream.Close();
        }

        public void Deserialize<T>(ref T obj)
        {
            ObjectStream = new FileStream(@"Arctium.dat", FileMode.Open);
            obj = (T)BinarySerializer.Deserialize(ObjectStream);
            ObjectStream.Close();
        }
    }
}
