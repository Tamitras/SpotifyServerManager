using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfHost.Interface
{
    [ServiceContract]
    public interface IWcfHost
    {

        [OperationContract]
        bool Register(string ipAdress, string username, out string message);


        [OperationContract]
        bool Exit(string ipAdress);
    }
}
