using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpcDaClient.Rcw;

namespace OpcDaClient
{
    public interface IServerFactory
    {
        IOpcServer CreateFromProgId(string progId);
    }
}
