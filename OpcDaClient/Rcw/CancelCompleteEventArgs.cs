using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.Rcw
{
    public class CancelCompleteEventArgs : EventArgs
    {
        public CancelCompleteEventArgs(int transactionId, int groupHandle)
        {
            TransactionId = transactionId;
            GroupHandle = groupHandle;
        }
        public int TransactionId { get; private set; }
        public int GroupHandle { get; private set; }
    }
}
