using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ClientServerTest.Core
{
    [Serializable]
    public class Packet
    {

        public string message;

        //public byte[] Serialize()
        //{
        //    BinaryFormatter bin = new BinaryFormatter();
        //    MemoryStream mem = new MemoryStream();
        //    bin.Serialize(mem, this);
        //    return mem.GetBuffer();
        //}

        //public Person DeSerialize(byte[] dataBuffer)
        //{
        //    BinaryFormatter bin = new BinaryFormatter();
        //    MemoryStream mem = new MemoryStream(dataBuffer, 0, dataBuffer.Length);
        //    return (Person)bin.Deserialize(mem);
        //}
    }
}
