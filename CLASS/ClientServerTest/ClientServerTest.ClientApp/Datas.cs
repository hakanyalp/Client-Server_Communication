using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerTest.ClientApp
{
    class Datas
    {
        public string Name { get; private set; }
        public int Age { get; private set; }
        public string Surname { get; private set; }

        private Datas() { }

        public Datas(string name, int age, string surname)
        {
            this.Name = name;
            this.Age = age;
            this.Surname = surname;
        }

        public byte[] Serialize()
        {
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            bin.Serialize(mem, this);
            return mem.GetBuffer();
        }

        public Status DeSerialize()
        {
            byte[] dataBuffer = TransmissionBuffer.ToArray();
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            mem.Write(dataBuffer, 0, dataBuffer.Length);
            mem.Seek(0, 0);
            return (Status)bin.Deserialize(mem);
        }
    }


}
