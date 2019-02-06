
//
//Created by Hakan Yavuzalp, Cemal Acar, Cansu Kaplan, Yunus Emre Kılıç
//

using ClientServerTest.Library;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace ClientServerTest.ClientApp
{
    public partial class ClientForm : Form
    {
        private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//Soket tanımlandı.
        private static byte[] _buffer = new byte[1024];//Gelen veri için byte tipinde array oluşturuldu.
        private int client_ID;

        public ClientForm()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                int attempts = 0;

                while (!_clientSocket.Connected)//_clientSocket bağlı değilse döngüye girmektedir.
                {
                    try
                    {
                        attempts++;
                        _clientSocket.Connect(new IPEndPoint(IPAddress.Parse(txIP.Text), Convert.ToInt32(txPort.Text)));// Connect fonksiyonuyla ıp adresi ve port numarası alınarak bağlantı sağlanmaktadır.
                    }
                    catch (SocketException)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            lblStatus.Text = "Trying connect: " + attempts.ToString();
                        });
                    }
                }
                this.Invoke((MethodInvoker)delegate
                {
                    lblStatus.Text = "Connected";
                });
                ReadServerMessage(_clientSocket);//Gelen mesajın okunması için metod çağırıldı.
            });
        }
        
        private void btnSend_Click(object sender, EventArgs e)
        {
            if (!txMessage.Text.StartsWith("@"))// Mesaj @ işareti ile başlamıyorsa mesaj server'a gönderilir.
            {
                NetworkStream networkStream = new NetworkStream(_clientSocket);

                Packet packet = new Packet();
                packet.message = txMessage.Text;
                packet.sender_ID = this.client_ID;
                packet.path = "client-server";

                //Class serialize ile binary formata dönüştürüldü.
                using (NetworkStream ns = new NetworkStream(_clientSocket))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(ms, packet);
                        byte[] buf = ms.ToArray();
                        ns.Write(buf, 0, buf.Length);//Serialize edilmiş class, gönderilir
                    }
                }
                listMessages.Items.Add("Client : " + packet.message);
            }
            else         // Başında @ işareti varsa diğer Client'a gönderme işlemini başlatır.
            {
                NetworkStream networkStream = new NetworkStream(_clientSocket);                

                Packet packet = new Packet();
                packet.message = txMessage.Text;
                packet.sender_ID = this.client_ID;
                packet.path = "client-client";

                string[] words = packet.message.Split(' ');
                try     // @ kısmında girilen sayıdan sonra boşluk girilmezse oluşacak hata için kullanılıyor
                {
                    packet.receiver_ID = Convert.ToInt32(words[0].Substring(1));
                    packet.message = words[1];
                    if (packet.receiver_ID != packet.sender_ID)
                    {
                        for (int i = 2; i < words.Count(); i++)
                        {
                            packet.message += " " + words[i];    // words[1] -> @'ten sonrasına denk geliyor, words[2] ilkine boşluk gelmemesi için döngüden önce yapılıyor
                        }
                        //Class serialize ile binary formata dönüştürüldü.
                        using (NetworkStream ns = new NetworkStream(_clientSocket))
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                BinaryFormatter formatter = new BinaryFormatter();
                                formatter.Serialize(ms, packet);
                                byte[] buf = ms.ToArray();
                                ns.Write(buf, 0, buf.Length);
                            }
                        }
                        listMessages.Items.Add("Client " + packet.sender_ID + " -> Client " + packet.receiver_ID + " : " + packet.message);
                    }
                    else
                    {
                        listMessages.Items.Add("You can't send message to yourself!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("You should write like @[client_id] [message] this format!");
                }
                
            }
            txMessage.Text = string.Empty;
        }

        private void ReadServerMessage(Socket client)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        byte[] temp = new byte[1024];
                        int len = client.Receive(temp);
                        Packet packet = new Packet();   // instance
                        
                        //Class deserialize ile ortak alandaki class formatına dönüştürüldü.
                        using (MemoryStream ms = new MemoryStream(temp, 0, len))
                        {
                            ms.Position = 0;
                            BinaryFormatter formatter = new BinaryFormatter();
                            packet = formatter.Deserialize(ms) as Packet;
                        }
                        
                        this.Invoke((MethodInvoker)delegate
                        {
                            if (packet.client_ID != -1)//client ID -1 değilse ilk kez bağlantı kuruluyor demektir. Serverdan gelen client Id bilgisi yukarıda tanımlanan değişkene atanır.
                            {
                                this.client_ID = packet.client_ID;
                                this.Text = "Client - " + this.client_ID;
                            }
                            else if (packet.path == "broadcast" || packet.path == "server-client")// Burada mesajın geldiği path'e göre ekrana yazdırılır.
                            {
                                listMessages.Items.Add("Server : " + packet.message);
                            }
                            else if (packet.path == "client-client")// Burada mesajın geldiği path'e göre ekrana yazdırılır.
                            {
                                listMessages.Items.Add("Client " + packet.sender_ID + " -> Client " + packet.receiver_ID + " : " + packet.message);
                            }
                            else
                            {
                                listMessages.Items.Add("ERROR in Read Client method");
                            }
                        });
                    }
                    catch (Exception ex)//Server erişim sağlanamazsa clientlar kill edilir.
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            Process[] _proceses = null;
                            _proceses = Process.GetProcessesByName("ClientServerTest.ClientApp");
                            foreach (Process proces in _proceses)
                            {
                                proces.Kill();
                            }

                            MessageBox.Show("Client Error : " + ex.Message);
                        });
                    }
                }
            });
        }

        
    }
}
