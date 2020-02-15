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

namespace WcfClient
{
    public partial class ClientForm : Form
    {
        private static IWcfHost Host = null;

        private bool CloseApplication = false;

        public IPAddress IPAddress { get; set; }
        public string HostName { get; set; }
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

        private void Init()
        {
            GetIpAdressAndHostname();

            new Thread(() =>
            {
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
            var ipAdress = ipAdresses.Where(c => c.Address.ToString().StartsWith("192")).FirstOrDefault();
#pragma warning restore CS0618 // Typ oder Element ist veraltet

            this.IPAddress = ipAdress;
        }

        private string GetHostName()
        {
            return Dns.GetHostName();
        }

        private void ConnectToSocketServer()
        {
            try
            {
                Int32 port = 1337;
                TcpClient client = new TcpClient("localhost", port);
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
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
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
            Uri baseAddress = new Uri("net.tcp://localhost:1338/Spotify");
            EndpointAddress address = new EndpointAddress(baseAddress);
            NetTcpBinding binding = new NetTcpBinding();
            ChannelFactory<IWcfHost> factory = new ChannelFactory<IWcfHost>(binding, address);
            Host = factory.CreateChannel();

            string message = string.Empty;
            try
            {
                Host.Register(this.IPAddress, this.HostName, out message);
                MessageBox.Show("Erfolgreich am Server registriert");
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
    }
}
