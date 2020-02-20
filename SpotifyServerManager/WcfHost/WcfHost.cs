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
using System.Web.Http;
using System.Web.Http.SelfHost;
using WcfHost.Interface;
using WcfHost.Models;

namespace WcfHost
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class WcfHost : IWcfHost
    {
        public bool CloseApplication { get; set; }
        private IPAddress IpAdress { get; set; }

        /// <summary>
        ///  Werden nur für die TCP Kommunikation verwendet
        /// </summary>
        private List<SocketMember> ListOfAllConnectedMember { get; set; }
        private TcpListener TcpListener { get; set; }
        private ServiceHost WcfServiceHost { get; set; }
        private SpotifyProvider SpotifyProvider { get; set; }
        private UdpClient UdpClient { get; set; }

        private HttpSelfHostServer SelfHostServer { get; set; }

        public static ServerVM ServerVM { get; set; } = new ServerVM();
        public WcfHost()
        {
            // Zur Verwendung von Spotify
            SpotifyProvider = new SpotifyProvider();
            SpotifyProvider.Connect();
        }

        /// <summary>
        /// Registriert den Benutzer am WCF Service
        /// </summary>
        /// <param name="ipAdress">Ip Adresse</param>
        /// <param name="hostname">Name des Rechners</param>
        /// <param name="message">Optionale Nachricht</param>
        /// <returns></returns>
        public bool Register(IPAddress ipAdress, string hostname, out string message)
        {
            message = string.Empty;

            ServerVM.AddUser(
                new WcfMember()
                {
                    Hostname = hostname,
                    IPAddress = ipAdress,
                    LoginDate = DateTime.Now
                });

            return true;
        }

        public async Task<string> PausePlay(IPAddress ipAdress, string hostname)
        {
            var member = ServerVM.ConnectedMembers.Where(c => c.IPAddress.Equals(ipAdress)).SingleOrDefault();
            string res = await SpotifyProvider.PerformPlayAsync();
            if (member != null)
            {
                ServerVM.WriteToLog($"{member.Hostname} hat Pause/Play gedrückt");
            }

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
                IpAdress = ipAdresses.FirstOrDefault();

                ServerVM.WriteToLog($"Server-Hostname: {Dns.GetHostName()}");
                ServerVM.WriteToLog($"Server-IpAdress: {IpAdress}");

                ListOfAllConnectedMember = new List<SocketMember>();

                StartListenForConnection();

                WcfServiceHost = new ServiceHost(typeof(WcfHost));
                Uri baseAddress = new Uri($"net.tcp://{IpAdress}:1338/Spotify");
                ServerVM.WriteToLog($"BaseAdress: {baseAddress}");
                NetTcpBinding binding = new NetTcpBinding();
                binding.Security.Mode = SecurityMode.None;
                binding.Security.Message.ClientCredentialType = MessageCredentialType.None;

                WcfServiceHost.AddServiceEndpoint(typeof(IWcfHost), binding, baseAddress);
                WcfServiceHost.Open();
                ServerVM.WriteToLog($"WCF-Service gestartet.\nPort: 1338");

                new Thread(() =>
                {
                    var config = new HttpSelfHostConfiguration("http://localhost:8082");
                    config.Routes.MapHttpRoute("API Default", "api/{controller}/{action}/{id}", new { id = RouteParameter.Optional });

                    SelfHostServer = new HttpSelfHostServer(config);
                    SelfHostServer.OpenAsync().Wait();

                    ServerVM.WriteToLog($"HTTP Server started....");
                }).Start();


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
            WcfServiceHost.Close();
            SelfHostServer.CloseAsync();
            // Benachrichtige alle Member, dass der Server geschlossen wird, also schließe auch alle Verbindungen.
        }

        private void StartListenForConnection()
        {
            new Thread(() =>
            {
                ServerVM.WriteToLog("Starte Thread für UDP Listening");
                WaitForClientRequest_UDP();
            }).Start();

            new Thread(() =>
            {
                ServerVM.WriteToLog("Starte Thread für TCP Listening");
                ListeningForConnectionTCP();
            }).Start();
        }

        private void ListeningForConnectionTCP()
        {
            try
            {
                TcpListener = new TcpListener(IPAddress.Any, 1337);
                TcpListener.Start();
                ServerVM.WriteToLog($"TCP-Server gestartet.\nPort: 1337");
                while (!CloseApplication)
                {
                    Thread.Sleep(20);
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

                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 1336);
                    UdpClient.Client.Bind(remoteEndPoint);
                    ServerVM.WriteToLog($"UdpClient wurde geöffnet: Port: {1336}");
                    Console.WriteLine(DateTime.Now + " >>>>>>>>>>> " + "Waiting for BroadcastMessages..." + "<<<<<<<<<<");
                    ServerVM.WriteToLog(" >>>>>>>>>>> " + "Waiting for BroadcastMessages..." + "<<<<<<<<<<");
                    myBuffer = UdpClient.Receive(ref remoteEndPoint);
                    Console.WriteLine(Environment.NewLine + seperator);
                    Console.WriteLine(DateTime.Now + " Eine Anfrage kam von: " + remoteEndPoint);
                    ServerVM.WriteToLog(" Eine Anfrage kam von: " + remoteEndPoint);

                    ASCIIEncoding encoder = new ASCIIEncoding();
                    String messageFromUdp = encoder.GetString(myBuffer);

                    messageFromUdp.Replace("\0", "");
                    messageFromUdp.Trim();

                    Console.WriteLine(DateTime.Now + " Empfanges Schlüsselwort: " + messageFromUdp.Trim() + " von: " + IpAdress.ToString());
                    ServerVM.WriteToLog(" Empfanges Schlüsselwort: " + messageFromUdp.Trim() + " von: " + IpAdress.ToString());
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
                        ServerVM.WriteToLog(" Gesendeter Broadcast: " + "\"" + IpAdress.ToString() + "\" " + "an: " + IpAdress.ToString());
                    }
                    else
                    {
                        Console.WriteLine(seperator);
                        Console.WriteLine(DateTime.Now + "Anfrage war ungültig");
                    }
                    Console.WriteLine(seperator + Environment.NewLine);
                    messageFromUdp = String.Empty;
                    UdpClient.Close();
                    ServerVM.WriteToLog("UdpClient wurde geschlossen");
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

            string msg = $"Willkommen <{tempMember.Name}>. Sie wurden vom Server erfasst";

            // Nachricht wird an Client gesendet
            //tempMember.SendToClient(msg);

            ServerVM.WriteToLog(msg);
        }
    }
}
