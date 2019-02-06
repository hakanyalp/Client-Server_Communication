using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientServerTest.ServerApp
{
    public partial class ServerForm : Form
    {
        public string response = string.Empty;

        // TODO 1 - Validasyonlar eklenecek

        private static Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static Dictionary<int, Socket> clientSockets = new Dictionary<int, Socket>();
        private static byte[] _buffer = new byte[1024];

        static int clientCounter = 1;

        // Thread signal.  
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public ServerForm()
        {
            InitializeComponent();
        }

        private void btnNewClient_Click(object sender, EventArgs e)
        {
            Process.Start("C:\\Users\\Hakan\\Downloads\\ClientServerTest\\ClientServerTest\\ClientServerTest.ClientApp\\bin\\Debug\\ClientServerTest.ClientApp.exe");
            
        }

        // 1. STEP Connection process start
        private void btnConnect_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = false;
            listMessages.Items.Add("Bağlantı başlatılıyor...");
            server.Bind(new IPEndPoint(IPAddress.Parse(textBox1.Text), Convert.ToInt32(textBox2.Text)));
            server.Listen(100);

            Task.Run(() =>
            {
                while (true)
                {
                    // Set the event to nonsignaled state.  
                    // allDone.Reset();
                    // server.BeginAccept(new AsyncCallback(AcceptCallback), server);

                    Socket client = server.Accept();
                    clientSockets.Add(clientCounter, client);

                    byte[] buffer = Encoding.UTF8.GetBytes(clientCounter.ToString());   // This two line for send client id for assign to client
                    Send(clientSockets[clientCounter], "client_ID=" + clientCounter.ToString());    // client_ID=1

                    this.Invoke((MethodInvoker)delegate
                    {
                        listMessages.Items.Add("Client" + Convert.ToString(clientCounter++) + " bağlandı");
                    });

                    ReadClientMessage(client);

                    // Wait until a connection is made before continuing.  
                    // allDone.WaitOne();
                }
            });
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string message = textBox3.Text;
            listMessages.Items.Add("Server : " + message);

            try
            {
                if (message.StartsWith("@"))
                {
                    int send_Number;
                    string[] words = message.Split(' ');
                    send_Number = Convert.ToInt32(words[0].Substring(1));    // substring delete first char
                    message = words[1];
                    for (int i = 2; i < words.Count(); i++)
                    {
                        message += " " + words[i];    // words[1] -> @'ten sonrasına denk geliyor, words[2] ilkine boşluk gelmemesi için döngüden önce yapılıyor
                    }
                    Send(clientSockets[send_Number], message);
                }
                else
                {
                    for (int i = 0; i < clientSockets.Count; i++)
                    {
                        Send(clientSockets[i + 1], message);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Client'a mesaj göndermek için -> @[Client ID] [mesaj] formatı kullanılmalıdır!");
            }


            //byte[] buffer = Encoding.UTF8.GetBytes(message);
            // server.Send(buffer);

            // clientSockets[0].Send(buffer);
            
            //if (clientCounter == 3)
            //    Send(clientSockets[2], message);
            //else
            //    Send(clientSockets[1], message);

            textBox3.Text = string.Empty;
        }

        private void AcceptCallback(IAsyncResult AR)
        {
            // Client ile bağlantı oluşturuldu, listeye eklenir.
            Socket client = server.EndAccept(AR);
            this.Invoke((MethodInvoker)delegate
            {
                string cli = "Client" + Convert.ToString(clientCounter) + " bağlandı";
                listMessages.Items.Add(cli);

            });

            client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), server);
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            this.Invoke((MethodInvoker)delegate
            {
                string cli = "bir bilgi geldi.";
            });
            Socket socket = (Socket)AR.AsyncState;
            // int received = socket.EndReceive(AR);


            byte[] dataBuf = new byte[1024];
            Array.Copy(_buffer, dataBuf, 1024);
            string text = Encoding.ASCII.GetString(dataBuf);

            this.Invoke((MethodInvoker)delegate
            {
                listMessages.Items.Add("Gelen mesaj : " + text);

                string req = textBox3.Text;

                byte[] buffer = Encoding.ASCII.GetBytes(req);
                // socket.Send(buffer);

            });


            byte[] data = Encoding.ASCII.GetBytes(response);
            //server.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            // server.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }

        /*private static void SendCallBack(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }*/


        private void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // handler.Send(byteData);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);

                this.Invoke((MethodInvoker)delegate
                {
                    listMessages.Items.Add(string.Format("Sent {0} bytes to client. ", bytesSent));
                });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReadClientMessage(Socket client)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    // NetworkStream networkStream = new NetworkStream(client);
                    try
                    {
                        // StreamReader reader = new StreamReader(networkStream);
                        // string message = reader.ReadLine();
                        byte[] temp = new byte[1024];

                        client.Receive(temp);

                        string message = Encoding.UTF8.GetString(temp);

                        if (message.StartsWith("@"))
                        {
                            int send_Number;
                            string[] words = message.Split(' ');
                            send_Number = Convert.ToInt32(words[0].Substring(1));    // substring delete first char
                            message = "@ " + words[1];  // başa konulan @, gönderilecek client'ın, mesajı farklı bir client'tan aldığını anlaması için kullanılıyor
                            for (int i = 2; i < words.Count(); i++)
                            {
                                message += " " + words[i];    // words[1] -> @'ten sonrasına denk geliyor, words[2] ilkine boşluk gelmemesi için döngüden önce yapılıyor
                            }
                            Send(clientSockets[send_Number], message);
                        }
                        else
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                listMessages.Items.Add("Client : " + message);
                            });
                        }

                    }
                    catch (Exception ex)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            listMessages.Items.Add("Error : " + ex.Message);
                        });
                    }
                }
            });
        }
    }
}
