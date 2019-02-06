
//
//Created by Hakan Yavuzalp, Cemal Acar, Cansu Kaplan, Yunus Emre Kılıç
//

using System;

namespace ClientServerTest.Library
{
    [Serializable]
    public class Packet
    {
        public string message = string.Empty;//İletilecek mesaj içeriğini saklar.
        public int client_ID = -1;//server ile client arasında bağlantı kurulduğunda serverdaki kayıtlı client ıd'sini clienta göndermeyi sağlar. 

        public string path;     //   server-client & client-server & client-client & broadcast işlemlerinden hangisinin yapılacağını belirler.
        public int sender_ID = -1;//gerekli olduğu durumlarda gönderici id'sini tutar.(client-client)

        public int receiver_ID = -1;//gerekli olduğu durumlarda alıcı id'sini tutar.(client-client & client-server)
    }
}
