using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.RcwWrapper
{
    public interface IOpcDataCallback : IDisposable
    {
        void OnDataChanged(int transactionId, OpcDataChanged[] values);
    }
}
