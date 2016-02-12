using OpcDaClient.RcwWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient
{
    public interface IServerFactory
    {
        IOpcServer CreateFromProgId(string progId);
    }
}
