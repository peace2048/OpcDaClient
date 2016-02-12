using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpcDaClient
{
    public sealed class LogManager
    {
        private static Func<Type, ILog> _getLogger = t => DebugLogger.Instance;
        public static Func<Type, ILog> GetLogger
        {
            get
            {
                return Interlocked.CompareExchange(ref _getLogger, t => null, null);
            }
            set
            {
                _getLogger = value;
            }
        }

        private class DebugLogger : ILog
        {
            public static readonly DebugLogger Instance = new DebugLogger();

            public void Debug(string message)
            {
                System.Diagnostics.Debug.Print("[DEGUB]" + message);
            }

            public void Error(string message)
            {
                System.Diagnostics.Debug.Print("[ERROR]" + message);
            }

            public void Error(string message, Exception exception)
            {
                System.Diagnostics.Debug.Print("[ERROR]" + message);
                System.Diagnostics.Debug.Print(exception.ToString());
            }

            public void Info(string message)
            {
                System.Diagnostics.Debug.Print("[INFO ]" + message);
            }

            public void Trace(string message)
            {
                System.Diagnostics.Debug.Print("[TRACE]" + message);
            }
        }
    }
}
