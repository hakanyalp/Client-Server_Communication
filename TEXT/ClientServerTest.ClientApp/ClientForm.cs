using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientServerTest.ClientApp
{
    public partial class ClientForm : Form
    {
        private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Thread receiveThread = null;
        private static byte[] _buffer = new byte[1024];
        private int client_ID;



        public ClientForm()
        {
            InitializeComponent();
            //receiveThread = new Thread(new ThreadStart(ReceiveDataFromServer));
            // receiveThread.Start();
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                int attempts = 0;

                while (!_clientSocket.Connected)
                {
                    try
                    {
                        attempts++;
                        _clientSocket.Connect(new IPEndPoint(IPAddress.Parse(textBox1.Text), Convert.ToInt32(textBox2.Text)));
                        // _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
                        ReadServerMessage(_clientSocket);
                    }
                    catch (SocketException)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            lblStatus.Text = "Bağlantı denemeleri: " + attempts.ToString();
                        });
                    }

                }
                this.Invoke((MethodInvoker)delegate
                {
                    lblStatus.Text = "Bağlandı";
                });
            });
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            // int received = socket.EndReceive(AR);


            byte[] dataBuf = new byte[1024];
            Array.Copy(_buffer, dataBuf, 1024);
            string text = Encoding.UTF8.GetString(dataBuf);

            this.Invoke((MethodInvoker)delegate
            {
                listMessages.Items.Add("Gelen mesaj : " + text);
            });


            // byte[] data = Encoding.ASCII.GetBytes(response);
            //server.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            // server.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }



        private void btnSend_Click(object sender, EventArgs e)
        {

            string message = Client_SendData_Text.Text;
            //byte[] buffer = Encoding.ASCII.GetBytes(req);

            if (!message.StartsWith("@"))   // this block provide send message Client to Server
            {
                listMessages.Items.Add("Client " + client_ID + " -> Server : " + message);
                Send(_clientSocket, message);
            }
            else if (message.StartsWith("@"))   // this block provide send message Client to Client
            {
                int receiver_Number;
                string[] words = message.Split(' ');
                receiver_Number = Convert.ToInt32(words[0].Substring(1));    // substring delete first char
                
                listMessages.Items.Add("Client " + client_ID + " -> Client " + receiver_Number + " : " + message);

                Send(_clientSocket, message + " " + this.client_ID);
            }

            // _clientSocket.Send(buffer)// ;

            Client_SendData_Text.Text = string.Empty;


            /* byte[] receivedBuf = new byte[1024];
             int rec = _clientSocket.Receive(receivedBuf);
             byte[] data = new byte[rec];
             Array.Copy(receivedBuf, data, rec);

             this.Invoke((MethodInvoker)delegate {
                 listMessages.Items.Add("Alınan Mesaj: " + Encoding.ASCII.GetString(data));
             });*/

        }


        private void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            handler.Send(byteData);

            // Begin sending the data to the remote device.  
            // handler.BeginSend(byteData, 0, byteData.Length, 0,
                // new AsyncCallback(SendCallback), handler);
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

        private void ReadServerMessage(Socket client)
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

                        this.Invoke((MethodInvoker)delegate
                        {
                            if (message.StartsWith("client_ID="))   // Assign the client_ID
                            {
                                this.client_ID = Convert.ToInt32(message.Substring(10));
                                this.Text = "Client - " + client_ID;
                            }
                            else if (message.StartsWith("@"))    // @2 mesaj_metni 1    // Client 1 : mesaj_metni
                            {
                                string[] words = message.Split(' ');
                                int sender_Client;
                                sender_Client = Convert.ToInt32(words[words.Count() - 1]);
                                message = words[1];
                                for (int i = 2; i < words.Count() - 1; i++)
                                {
                                    message += " " + words[i];    // words[1] -> @'ten sonrasına denk geliyor, words[2] ilkine boşluk gelmemesi için döngüden önce yapılıyor
                                }
                                listMessages.Items.Add("Client " + sender_Client + " : " + message);
                            }
                            else
                            {
                                listMessages.Items.Add("Server : " + message);
                            }
                        });

                    }
                    catch (Exception ex)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            listMessages.Items.Add("Client Error : " + ex.Message);
                        });
                    }
                }
            });
        }
    }
}
