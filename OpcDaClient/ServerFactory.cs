using OpcDaClient.Rcw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient
{
    public class ServerFactory : IServerFactory
    {
        public IOpcServer CreateFromProgId(string progId)
        {
            using (var factory = new Opc.Ua.Com.ServerFactory())
            {
                factory.Connect();
                try
                {
                    return new OpcServer(factory.CreateServer(new Uri("opc.com://localhost/" + progId), null));
                }
                finally
                {
                    factory.Disconnect();
                }
            }
        }
    }
}
