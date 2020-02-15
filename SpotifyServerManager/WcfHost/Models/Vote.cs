using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfHost.Models
{
    public class Vote
    {
        public string SongName { get; set; }

        public TcpMember VotetBy { get; set; }

        public DateTime VoteDate { get; set; }

        public Vote()
        {

        }
    }
}
