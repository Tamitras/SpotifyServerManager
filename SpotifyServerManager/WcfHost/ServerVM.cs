using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfHost
{
    public class ServerVM
    {
        public List<string> ConnectedUsers { get; set; }

        public delegate void UserHasConnectedDelegate(string user);
        public delegate void LogHasChangedDelegate(string msg);

        public event UserHasConnectedDelegate UserHasConnected;
        public event UserHasConnectedDelegate UserHasDisconected;
        public event LogHasChangedDelegate LogHasChanged;

        public ServerVM()
        {
            this.ConnectedUsers = new List<string>();
        }

        public void AddUser(string userCredentials)
        {
            this.ConnectedUsers.Add(userCredentials);
            this.UserHasConnected?.Invoke(userCredentials);
        }

        public void RemoveUser(string ipadress)
        {
            var userToDelete = this.ConnectedUsers.Where(c => c.Contains(ipadress)).SingleOrDefault();
            if(userToDelete != null)
            {
                this.ConnectedUsers.Remove(userToDelete);
                this.UserHasDisconected?.Invoke(ipadress);
                this.WriteToLog(userToDelete + " hat keine Lust mehr und ist gegangen");
            }
        }

        public void WriteToLog(string s)
        {
            this.LogHasChanged?.Invoke(s);
        }

    }
}
