using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.RcwWrapper
{
    public interface IOpcServer : IDisposable
    {
        IOpcGroup AddGroup(string name, bool isActive, int updateRate, float deadband, int localeId);
    }
}
