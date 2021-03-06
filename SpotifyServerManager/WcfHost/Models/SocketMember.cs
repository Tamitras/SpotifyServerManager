﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WcfHost.Models
{
    public class SocketMember
    {
        public String Name { get; set; }
        public EndPoint EndPoint { get; set; }
        public TcpClient TcpClient { get; set; }
        public DateTime LoginTime { get; set; }


        public SocketMember(TcpClient client)
        {
            TcpClient = client;
            EndPoint = client.Client.LocalEndPoint;
        }

        public void SendToClient(string msg)
        {
            try
            {
                ASCIIEncoding encoder = new ASCIIEncoding();
                // Begrüßung des Members
                Byte[] bufferToClient = encoder.GetBytes(msg);

                if (this.TcpClient.Connected)
                {
                    // Nachricht wird an Client gesendet
                    this.TcpClient.GetStream().Write(bufferToClient, 0, bufferToClient.Length);
                    this.TcpClient.GetStream().Flush();
                }
                else
                {
                    Console.WriteLine("Client not connected");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Client not connected");
            }
        }

        public void Close()
        {
            this.TcpClient.GetStream().Close();
        }
    }
}
