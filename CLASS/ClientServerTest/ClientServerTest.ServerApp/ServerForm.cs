
//
//Created by Hakan Yavuzalp, Cemal Acar, Cansu Kaplan, Yunus Emre Kılıç
//

using ClientServerTest.Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientServerTest.ServerApp
{
    public partial class ServerForm : Form
    {
        public string response = string.Empty;

        // TODO 1 - Validasyonlar eklenecek

        private static Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//Soket tanımlandı.
        private static Dictionary<int, Socket> clientSockets = new Dictionary<int, Socket>();//Clientlar için dictionary array oluşturuldu.
        private static byte[] _buffer = new byte[1024]; //Gelen veri için byte tipinde array oluşturuldu.

        static int clientCounter = 1;

        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public ServerForm()
        {
            InitializeComponent();
        }

        private void btnNewClient_Click(object sender, EventArgs e)
        {
            //Yeni Client Oluştur butonuna tıklayınca, görev yöneticisinden halihazırda çalışan process'lere bakıp, Client Form'un path'ini alıyor.Ardından yenniden çalıştırıyor.
            foreach (Process PPath in Process.GetProcesses())
            {
                if (PPath.ProcessName.ToString() == "ClientServerTest.ClientApp")
                {
                    string fullpath = PPath.MainModule.FileName;
                    Process.Start(fullpath);
                    break;
                }
            }
        }

        // Bağlan butonu
        private void btnConnect_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = false;
            listMessages.Items.Add("Bağlantı başlatılıyor...");
            server.Bind(new IPEndPoint(IPAddress.Parse(txIP.Text), Convert.ToInt32(txPort.Text)));//IP adresi ve port numarası alarak soket oluşturur.
            server.Listen(100);// Oluşturulan soketi dinler.

            Task.Run(() =>
            {
                while (true)
                {
                    Socket client = server.Accept();//Client tarafından gelen bağlantıyı kabul eder.
                    clientSockets.Add(clientCounter, client);//Gelen client'ı clietSockets dictionary arrayine ekler.
                    // client id ataması yapar.
                    NetworkStream networkStream = new NetworkStream(clientSockets[clientCounter]);

                    Packet packet = new Packet();//Paket nesnesi oluşturuldu.
                    packet.client_ID = clientCounter;// Client counteri paket nesnesi içerisindeki client_ID içine yazıldı.

                    //Class serialize ile binary formata dönüştürüldü.
                    using (NetworkStream ns = new NetworkStream(clientSockets[clientCounter]))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            formatter.Serialize(ms, packet);
                            byte[] buf = ms.ToArray();
                            ns.Write(buf, 0, buf.Length);
                        }
                    }

                    this.Invoke((MethodInvoker)delegate
                    {
                        lblStatus.Text = "Bağlandı";
                        listClients.Items.Add(clientCounter);
                        listMessages.Items.Add("Client " + clientCounter++.ToString() + " bağlandı");
                    });

                    ReadClientMessage(client);//Gelen mesajın okunması için metod çağırıldı.
                }
            });
        }

        //Gönder butonu
        private void btnSend_Click(object sender, EventArgs e)
        {
            OpenSSL.Crypto.RSA rsa = new OpenSSL.Crypto.RSA();
            rsa.GenerateKeys(1024, 65537, null, null);

            File.WriteAllText("MasterPrivateKey.pem", rsa.PrivateKeyAsPEM);
            File.WriteAllText("MasterPublicKey.pem", rsa.PublicKeyAsPEM);

            Packet packet = new Packet();
            packet.message = txMessage.Text;
            if (!txMessage.Text.StartsWith("@"))// Mesaj @ işareti ile başlamıyorsa broadcast yapılacaktır.
            {
                packet.path = "broadcast";
                for (int i = 1; i < clientSockets.Count + 1; i++)//Broadcast yapılması için dictionarydeki tüm clientlara mesaj gönderilmektedir.
                {
                    //Class serialize ile binary formata dönüştürüldü.
                    using (NetworkStream ns = new NetworkStream(clientSockets[i]))//Tüm soketlere gönderilmesi için clientSockets[i] işlemi gerçekleştirildi.
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            formatter.Serialize(ms, packet);
                            byte[] buf = ms.ToArray();
                            ns.Write(buf, 0, buf.Length);//Serialize edilmiş class gönderilir
                        }
                    }
                }
                listMessages.Items.Add("Broadcast : " + txMessage.Text);
            }
            else//mesaj @ işareti ile başlıyorsa tek clienta yönlendirme yapılacaktır.
            {
                packet.message = txMessage.Text;
                try
                {
                    string[] words = packet.message.Split(' ');
                    packet.receiver_ID = Convert.ToInt32(words[0].Substring(1));//words[0]'da hedef client ıd bulunur. Substring ile başındaki @ işareti kaldırılarak atanır.
                    if (clientSockets.ContainsKey(packet.receiver_ID))      // Client var mı diye kontrol ediyor
                    {
                        packet.path = "server-client";
                        packet.message = words[1];//@ işaretinden sonraki kelime alınır. Sonrasındaki kelimeler aşağıdaki for döngüsünde boşluklu şekilde eklenir.
                        for (int i = 2; i < words.Length - 1; i++)
                        {
                            packet.message += " " + words[i];    // words[1] -> @'ten sonrasına denk geliyor, words[2] ilkine boşluk gelmemesi için döngüden önce yapılıyor
                        }
                        using (NetworkStream ns = new NetworkStream(clientSockets[packet.receiver_ID]))
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                BinaryFormatter formatter = new BinaryFormatter();
                                formatter.Serialize(ms, packet);
                                byte[] buf = ms.ToArray();
                                ns.Write(buf, 0, buf.Length);
                            }
                        }
                        listMessages.Items.Add("Client " + packet.receiver_ID + " : " + txMessage.Text);
                    }
                    else// Client dictionary array'inde yoksa
                    {
                        listMessages.Items.Add("There is no such client");
                    }
                }
                catch (Exception ex)// @ ifadesinden sonra sayı dışında farklı veri girilirse buraya girer.
                {
                    MessageBox.Show("You should write like @[client_id] [message] this format!");
                }
            }
            txMessage.Text = string.Empty;
        }

        //Socketden gelen mesaj buraya girmektedir.
        private void ReadClientMessage(Socket client)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        byte[] temp = new byte[1024];
                        int len = client.Receive(temp);
                        Packet packet = new Packet();

                        //Class deserialize ile ortak alandaki class formatına dönüştürüldü.
                        using (MemoryStream ms = new MemoryStream(temp, 0, len))
                        {
                            ms.Position = 0;
                            BinaryFormatter formatter = new BinaryFormatter();
                            packet = formatter.Deserialize(ms) as Packet;
                        }

                        this.Invoke((MethodInvoker)delegate
                        {
                            if (packet.path == "client-client")
                            {
                                if (clientSockets.ContainsKey(packet.receiver_ID))     // alıcı client varsa kontrolünü de sağlıyor.
                                {
                                    using (NetworkStream ns = new NetworkStream(clientSockets[packet.receiver_ID]))
                                    {
                                        using (MemoryStream ms = new MemoryStream())
                                        {
                                            BinaryFormatter formatter = new BinaryFormatter();
                                            formatter.Serialize(ms, packet);
                                            byte[] buf = ms.ToArray();
                                            ns.Write(buf, 0, buf.Length);
                                        }
                                    }
                                }
                                else    // eğer gönderici clienta hata mesajı gönderiyor
                                {
                                    packet.message = "There is no such client";
                                    packet.path = "server-client";
                                    using (NetworkStream ns = new NetworkStream(clientSockets[packet.sender_ID]))
                                    {
                                        using (MemoryStream ms = new MemoryStream())
                                        {
                                            BinaryFormatter formatter = new BinaryFormatter();
                                            formatter.Serialize(ms, packet);
                                            byte[] buf = ms.ToArray();
                                            ns.Write(buf, 0, buf.Length);
                                        }
                                    }
                                }
                            }
                            else if (packet.path == "client-server")
                            {
                                listMessages.Items.Add("Client " + packet.sender_ID + " : " + packet.message);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            listMessages.Items.Add("Error : " + ex.Message);
                        });
                        break;
                    }
                }
            });
        }

    }
}
