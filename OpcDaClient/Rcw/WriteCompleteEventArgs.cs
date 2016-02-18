using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.Rcw
{
    public class WriteCompleteEventArgs : EventArgs
    {
        public WriteCompleteEventArgs(
            int transactionId,
            int groupHandle,
            int errorCode,
            int itemCount,
            int[] itemHandles,
            int[] itemErrors)
        {
            TransactionId = transactionId;
            GroupHandle = groupHandle;
            MasterErrorCode = errorCode;
            ItemCount = itemCount;
            ItemClientHandles = itemHandles;
            ItemErrors = itemErrors;
        }
        public int TransactionId { get; private set; }
        public int GroupHandle { get; private set; }
        public int MasterErrorCode { get; private set; }
        public int ItemCount { get; private set; }
        public int[] ItemClientHandles { get; private set; }
        public int[] ItemErrors { get; private set; }
    }
}
