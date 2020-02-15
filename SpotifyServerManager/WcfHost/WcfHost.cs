using SpotifyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WcfHost.Interface;
using WcfHost.Models;

namespace WcfHost
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class WcfHost : IWcfHost
    {
        public bool CloseApplication { get; set; }
        private IPAddress IpAdress { get; set; }
        private List<SocketMember> ListOfAllConnectedMember { get; set; }
        private TcpListener TcpListener { get; set; }
        private ServiceHost Host { get; set; }

        private SpotifyProvider SpotifyProvider { get; set; }

        private UdpClient UdpClient { get; set; }

        public static ServerVM ServerVM { get; set; } = new ServerVM();
        public WcfHost()
        {
            SpotifyProvider = new SpotifyProvider();
            SpotifyProvider.Connect();
        }

        public bool Register(IPAddress ipAdress, string username, out string message)
        {
            Console.WriteLine($"Benutzer mit der IP: {ipAdress} hat sich erfolgreich am Server registriert");
            message = "Hier könnte ihre Werbung stehen";

            ServerVM.AddUser(
                new TcpMember()
                {
                    Hostname = username,
                    IPAddress = ipAdress,
                    LoginDate = DateTime.Now
                });

            return true;
        }

        public async Task<string> PausePlay(IPAddress ipAdress, string hostname)
        {
            string res = await SpotifyProvider.PerformPlayAsync();
            return res;
        }

        /// <summary>
        /// Entfernt User mit IPAdresse
        /// </summary>
        /// <param name="ipAdress"></param>
        /// <returns></returns>
        public bool Exit(IPAddress ipAdress)
        {
            bool ret = true;
            try
            {
                ServerVM.RemoveUser(ipAdress);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

                var adresses = Dns.GetHostAddresses(Dns.GetHostName());
                var ipAdresses = adresses.Where(c => c.AddressFamily == AddressFamily.InterNetwork).ToList();
                IpAdress = ipAdresses.Where(c => c.Address.ToString().StartsWith("192")).FirstOrDefault();

                ListOfAllConnectedMember = new List<SocketMember>();
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


        public void VoteSkip(IPAddress ipAdress, string hostname)
        {
            // Benachrichtige alle User bis auf den User, der getriggered hat
            // Setze Vote-Status auf in Progress/Idle --> So dass kein weiterer Vote angestoßen werden kann
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            foreach (var sMember in ListOfAllConnectedMember)
            {
                sMember.SendToClient("Close");
            }

            Console.WriteLine("exit");
            UdpClient.Close();
            Host.Close();
            // Benachrichtige alle Member, dass der Server geschlossen wird, also schließe auch alle Verbindungen.
        }

        private void StartListenForConnection()
        {
            new Thread(() =>
            {
                WaitForClientRequest_UDP();
            }).Start();

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
                            SocketMember member = new SocketMember(client);
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

        /// <summary>
        /// Methode die in einem eigenen Thread läuft
        /// Wartet auf Anfrage aus dem Netzwerk
        /// Sendet bei erfolgreicher Anfrage die IPAdresse in das Netzwerk
        /// </summary>
        private void WaitForClientRequest_UDP()
        {
            const String seperator = "-------------------------------------------------------------------------";
            try
            {
                while (!CloseApplication)
                {
                    UdpClient = new UdpClient();
                    Byte[] myBuffer = new Byte[4096];

                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 1337);
                    UdpClient.Client.Bind(remoteEndPoint);
                    Console.WriteLine(DateTime.Now + " >>>>>>>>>>> " + "Waiting for BroadcastMessages..." + "<<<<<<<<<<");
                    myBuffer = UdpClient.Receive(ref remoteEndPoint);
                    Console.WriteLine(Environment.NewLine + seperator);
                    Console.WriteLine(DateTime.Now + " Eine Anfrage kam von: " + remoteEndPoint);

                    ASCIIEncoding encoder = new ASCIIEncoding();
                    String messageFromUdp = encoder.GetString(myBuffer);

                    messageFromUdp.Replace("\0", "");
                    messageFromUdp.Trim();

                    Console.WriteLine(DateTime.Now + " Empfanges Schlüsselwort: " + messageFromUdp.Trim() + " von: " + IpAdress.ToString());
                    Byte[] bufferToClient = new Byte[0];
                    if (messageFromUdp.Contains("<Server>")) //Server-Anfrage
                    {
                        Thread.Sleep(1000);
                        bufferToClient = encoder.GetBytes(IpAdress.ToString());
                        UdpClient.Send(bufferToClient, bufferToClient.Length, remoteEndPoint);
                        Console.WriteLine(DateTime.Now + " Gesendeter Broadcast: "
                            + "\""
                            + IpAdress.ToString()
                            + "\" "
                            + "an: "
                            + IpAdress.ToString());
                    }
                    else
                    {
                        Console.WriteLine(seperator);
                        Console.WriteLine(DateTime.Now + "Anfrage war ungültig");
                    }
                    Console.WriteLine(seperator + Environment.NewLine);
                    messageFromUdp = String.Empty;
                    UdpClient.Close();
                }
            }
            catch (Exception ex)
            {
                //Hier ging etwas schief :/
            }
        }

        private void HandleClientComm(Object client) // Der Delegate erwartet einen Typ "Object"
        {
            TcpClient Client = (TcpClient)client; // Diesen casten wir in "TcpClient"
            SocketMember tempMember = ListOfAllConnectedMember.SingleOrDefault(c => c.TcpClient == Client);

            if (tempMember.Name.Contains('\0'))
            {
                tempMember.Name = tempMember.Name.Split('\0').First();
            }

            string msg = $"<{DateTime.Now}> Willkommen <{tempMember.Name}>. Sie wurden vom Server erfasst";

            // Nachricht wird an Client gesendet
            //tempMember.SendToClient(msg);

            Thread.Sleep(50);
            Console.WriteLine($"<{tempMember.Name}> hat sich angemeldet");
            ServerVM.WriteToLog($"<{tempMember.Name}> hat sich angemeldet");
        }


    }
}
