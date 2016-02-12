using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient
{
    public interface ILog
    {
        void Error(string message);
        void Error(string message, Exception exception);
        void Info(string message);
        void Debug(string message);
        void Trace(string message);
    }
}
