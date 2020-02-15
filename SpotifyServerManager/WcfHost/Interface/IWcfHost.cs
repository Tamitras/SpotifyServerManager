using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfHost.Interface
{
    [ServiceContract]
    public interface IWcfHost
    {

        [OperationContract]
        bool Register(IPAddress ipAdress, string hostname, out string message);


        [OperationContract]
        bool Exit(IPAddress ipAdress);

        [OperationContract]
        void VoteSkip(IPAddress ipAdress, string hostname);

        [OperationContract]
        Task<string> PausePlay(IPAddress ipAdress, string hostname);
    }
}
