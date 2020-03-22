using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WcfHost.Models;

namespace WcfHost
{
    public class ServerVM
    {
        public List<WcfMember> ConnectedMembers { get; set; }

        public delegate void LogHasChangedDelegate(string msg);
        public event LogHasChangedDelegate LogHasChanged;

        public delegate void MemberHasChangedDelegate();
        public event MemberHasChangedDelegate MemberHasChanged;

        public ServerVM()
        {
            this.ConnectedMembers = new List<WcfMember>();
        }

        public void AddUser(WcfMember member)
        {
            this.ConnectedMembers.Add(member);
            this.MemberHasChanged();
        }

        public void RemoveUser(IPAddress ipadress)
        {
            var userToDelete = this.ConnectedMembers.Where(c => c.IPAddress.Equals(ipadress)).SingleOrDefault();
            if(userToDelete != null)
            {
                this.ConnectedMembers.Remove(userToDelete);
                this.MemberHasChanged();
                this.WriteToLog(userToDelete.Hostname + " hat keine Lust mehr und ist gegangen");
            }
        }

        public void WriteToLog(string s)
        {
            this.LogHasChanged?.Invoke(s);
        }
    }
}
