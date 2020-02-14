using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Threading;
using WcfHost.Interface;
using WcfHost.Models;

namespace WcfHost
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class WcfHost : IWcfHost
    {
        public bool CloseApplication { get; set; }
        private IPAddress IpAdress { get; set; }
        private List<Member> ListOfAllConnectedMember { get; set; }
        private TcpListener TcpListener { get; set; }
        private ServiceHost Host { get; set; }

        public static ServerVM ServerVM { get; set; } = new ServerVM();
        public WcfHost() { }

        public bool Register(string ipAdress, string username, out string message)
        {
            Console.WriteLine($"Benutzer mit der IP: {ipAdress} hat sich erfolgreich am Server registriert");
            message = "Hier könnte ihre Werbung stehen";

            ServerVM.AddUser($"{username} | <{ipAdress}>");

            return true;
        }

        /// <summary>
        /// Entfernt User mit IPAdresse
        /// </summary>
        /// <param name="ipAdress"></param>
        /// <returns></returns>
        public bool Exit(string ipAdress)
        {
            bool ret = true;
            try
            {
                ServerVM.RemoveUser(ipAdress);
            }
            catch (Exception ex)
            {
                ret = false;
            }
            return ret;
        }

        public bool Start(out string message)
        {
            bool ret = true;
            message = string.Empty;

            try
            {
                AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
                IpAdress = Dns.GetHostByName(Dns.GetHostName()).AddressList[0];
                ListOfAllConnectedMember = new List<Member>();
                TcpListener = new TcpListener(IPAddress.Any, 1337);
                StartListenForConnection();

                Host = new ServiceHost(typeof(WcfHost));
                Uri baseAddress = new Uri("net.tcp://localhost:1338/Spotify");
                NetTcpBinding binding = new NetTcpBinding();
                Host.AddServiceEndpoint(typeof(IWcfHost), binding, baseAddress);
                Host.Open();

                ServerVM.WriteToLog("Wcf Dienst gestartet");
                ServerVM.WriteToLog("Server wurde gestartet");
                ServerVM.WriteToLog("Server horcht auf Port: " + TcpListener.LocalEndpoint);

            }
            catch (Exception ex)
            {
                message = ex.Message;
                ret = false;
            }

            return ret;
        }


        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            foreach (var member in ListOfAllConnectedMember)
            {
                member.SendToClient("Close");
            }

            Console.WriteLine("exit");
            Host.Close();
            // Benachrichtige alle Member, dass der Server geschlossen wird, also schließe auch alle Verbindungen.
        }

        private void StartListenForConnection()
        {
            new Thread(() =>
            {
                ListeningForConnection();
            }).Start();
        }

        private void ListeningForConnection()
        {
            TcpListener.Start();
            try
            {
                while (!CloseApplication)
                {
                    Thread.Sleep(100);
                    //blocks until a client has connected to the server
                    if (TcpListener.Pending())
                    {
                        TcpClient client = TcpListener.AcceptTcpClient();
                        if (null != client)
                        {
                            Member member = new Member(client);
                            NetworkStream clientStream = client.GetStream();

                            byte[] message = new byte[4096]; // Dies ist unser Buffer
                            int bytesRead;
                            bytesRead = clientStream.Read(message, 0, 4096); // Hier warten wir darauf bis der Client uns Daten sendet - diese werden in einem ByteArray gespeichert

                            if (bytesRead > 0)
                            {
                                String receivedMessage = new ASCIIEncoding().GetString(message);

                                member.Name = receivedMessage;
                                member.LoginTime = DateTime.Now;
                            }
                            ListOfAllConnectedMember.Add(member);
                        }

                        //create a thread to handle communication 
                        //with connected client
                        Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                        clientThread.Start(client);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = "Ein Fehler ist aufgetreten: " + ex.Message;
                Console.WriteLine(msg);
                ServerVM.WriteToLog(msg);
            }
            TcpListener.Stop();
        }

        private void HandleClientComm(Object client) // Der Delegate erwartet einen Typ "Object"
        {
            TcpClient Client = (TcpClient)client; // Diesen casten wir in "TcpClient"
            Member tempMember = ListOfAllConnectedMember.SingleOrDefault(c => c.TcpClient == Client);

            if (tempMember.Name.Contains('\0'))
            {
                tempMember.Name = tempMember.Name.Split('\0').First();
            }

            string msg = $"<{DateTime.Now}> Willkommen <{tempMember.Name}>. Sie wurden vom Server erfasst";

            // Nachricht wird an Client gesendet
            tempMember.SendToClient(msg);

            Thread.Sleep(50);
            Console.WriteLine($"<{tempMember.Name}> hat sich angemeldet");
            ServerVM.WriteToLog($"<{tempMember.Name}> hat sich angemeldet");
        }
    }
}
