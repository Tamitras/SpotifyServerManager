using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfClient.UDP
{
    public class UDPMessage
    {
        /// <summary>
        /// Die Daten die Übertragen werden sollen
        /// </summary>
        public String Data { get; set; }
        public Byte[] DataInBytes { get; set; }

        /// <summary>
        /// Der Port über den die Übertragung stattfindet
        /// </summary>
        public Int32 Port { get; set; }

        public String TargetAddress { get; set; }

        private ASCIIEncoding Encoder { get; set; }

        public UDPMessage()
        {
            Data = "<Server>";
            Port = 1336;
            TargetAddress = "255.255.255.255";
            Encoder = new ASCIIEncoding();
            DataInBytes = new Byte[4096];
            DataInBytes = Encoder.GetBytes(Data);
        }

        public String ReadBytes(Byte[] bytes)
        {
            if (bytes.Length > 0)
            {
                return Encoder.GetString(bytes);
            }
            else
            {
                return null;
            }
        }
    }
}
