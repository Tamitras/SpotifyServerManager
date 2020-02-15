using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WcfHost.Interface;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using WcfClient.UDP;

namespace WcfClient
{
    public partial class ClientForm : Form
    {
        /// <summary>
        /// Ipadresse des Clients
        /// </summary>
        public IPAddress IPAddress { get; set; }

        /// <summary>
        /// Hostname des Clients
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gibt an ob der Client mit einem lokalen Server verbunden ist
        /// </summary>
        public Boolean IsLocalServer { get; set; }

        /// <summary>
        /// IpAdresse des HauptServers
        /// </summary>
        public String ConnectionString { get; set; }

        private static IWcfHost Host = null;
        private bool CloseApplication = false;


        public ClientForm()
        {
            InitializeComponent();
            this.Init();

            Application.ApplicationExit += Application_ApplicationExit;
            this.Disposed += ClientForm_Disposed;
        }

        private void ClientForm_Disposed(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            if (Host != null)
            {
                try
                {
                    Host.Exit(this.IPAddress);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    // Server wurde geschlossen!
                }
            }

            CloseApplication = true;
            Application.Exit();
        }

        /// <summary>
        /// Init-Methode
        /// </summary>
        public void GetNetworkServerAdress()
        {
            UDPMessage message = new UDPMessage();
            UdpClient udpClient = new UdpClient();
            udpClient.Client.ReceiveTimeout = 5000;
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 1337);
            BroadCastSend(message, udpClient, remoteEndPoint);
        }



        private void Init()
        {
            GetIpAdressAndHostname();

            new Thread(() =>
            {
                this.GetNetworkServerAdress();
                this.ConnectToSocketServer();
            }).Start();

            new Thread(() =>
            {
                this.ConnectToTcpServer();
            }).Start();
        }

        private void GetIpAdressAndHostname()
        {
            this.HostName = Dns.GetHostName(); // Retrive the Name of HOST  
            //string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            var hostEntry = Dns.GetHostEntry(HostName);
            var ipAdresses = hostEntry.AddressList.Where(c => c.AddressFamily == AddressFamily.InterNetwork).ToList();

#pragma warning disable CS0618 // Typ oder Element ist veraltet
            var ipAdress = ipAdresses.FirstOrDefault();
#pragma warning restore CS0618 // Typ oder Element ist veraltet

            this.IPAddress = ipAdress;
        }

        private void ConnectToSocketServer()
        {
            try
            {
                Int32 port = 1337;
                IPEndPoint serverAddress = new IPEndPoint(IPAddress.Parse(this.ConnectionString), port);
                TcpClient client = new TcpClient();
                client.Connect(serverAddress);

                NetworkStream stream = client.GetStream();
                ASCIIEncoding encoder = new ASCIIEncoding();

                Thread.Sleep(20);

                // Überlieferung des Hostnames
                Byte[] bufferToClient = encoder.GetBytes(this.HostName);
                client.GetStream().Write(bufferToClient, 0, bufferToClient.Length);
                client.GetStream().Flush();

                Thread.Sleep(20);

                while (!CloseApplication)
                {
                    WaitForServerMessages(client, stream);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
        }

        private void WaitForServerMessages(TcpClient client, NetworkStream stream)
        {
            Thread.Sleep(200);

            if (stream.DataAvailable
                && client.Connected)
            {
                byte[] message = new byte[4096]; // Dies ist unser Buffer
                int bytesRead;
                bytesRead = stream.Read(message, 0, 4096);

                if (bytesRead > 0)
                {
                    String receivedMessage = new ASCIIEncoding().GetString(message);
                    if (receivedMessage.Contains("Close"))
                    {
                        client.Close();
                        stream.Close();
                        CloseApplication = true;
                        Application.Exit();
                    }
                }
            }
        }

        private void ConnectToTcpServer()
        {
            while (string.IsNullOrEmpty(ConnectionString))
            {
                Thread.Sleep(1000);
            }

            Thread.Sleep(1000);

            Uri baseAddress = new Uri($"net.tcp://{ConnectionString}:1338/Spotify");
            EndpointAddress address = new EndpointAddress(baseAddress);
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;

            ChannelFactory<IWcfHost> factory = new ChannelFactory<IWcfHost>(binding, address);
            Host = factory.CreateChannel();

            string message = string.Empty;
            try
            {
                if (Host.Register(this.IPAddress, this.HostName, out message))
                {
                    MessageBox.Show("Erfolgreich am Server registriert");
                }
                else
                {
                    MessageBox.Show("MÖÖÖÖÖG");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void buttonStartStop_Click(object sender, EventArgs e)
        {
            this.textBoxCurrentSongName.Text = await Host.PausePlay(this.IPAddress, this.HostName);
        }

        /// <summary>
        /// Sendet einen Broadcast ins Netzwerk
        /// Wenn einer darauf antwortet, dann mit seiner IPAdresse
        /// </summary>
        /// <returns></returns>
        private void BroadCastSend(UDPMessage udpMessage, UdpClient udpClient, IPEndPoint remoteEndPoint)
        {
            IsLocalServer = true;
            try
            {
                //Broadcast wird versendet
                udpClient.Send(udpMessage.DataInBytes, udpMessage.Data.Length, udpMessage.TargetAddress, udpMessage.Port);
                try
                {
                    //Daten werden aus dem udpStream gelesen
                    udpMessage.DataInBytes = udpClient.Receive(ref remoteEndPoint);
                    String messageFromServer = udpMessage.ReadBytes(udpMessage.DataInBytes);

                    if (null != messageFromServer)
                    {
                        ConnectionString = messageFromServer;
                        udpClient.Close();
                    }
                    else //Keine Antwort zurück bekommen
                    {
                        // Server im Internet wird verwendet
                        udpClient.Close();
                        udpClient.Client.Disconnect(true);
                        // Wenn man die Verbindung über das Internet laufen lassen möchte
                        //ConnectionString = Properties.Settings.Default.ConnectionString;
                        //IsLocalServer = false;
                    }
                }
                catch (Exception timeout)
                {
                    udpClient.Close();
                    // Wenn man die Verbindung über das Internet laufen lassen möchte
                    //ConnectionString = Properties.Settings.Default.ConnectionString;
                    IsLocalServer = false;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message); //TODO:
            }
        }
    }
}
