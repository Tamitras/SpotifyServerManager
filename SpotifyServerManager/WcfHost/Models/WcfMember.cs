using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WcfHost.Models
{
    public class WcfMember
    {
        public IPAddress IPAddress { get; set; }
        public string Hostname  { get; set; }

        public int VotingsRequestet { get; set; }

        public DateTime LoginDate { get; set; }

        public WcfMember()
        {

        }
    }
}
