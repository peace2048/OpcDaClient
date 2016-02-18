using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.Rcw
{
    public class ReadCompleteEventArgs : EventArgs
    {
        public ReadCompleteEventArgs(
            int transactionId,
            int groupHandle,
            int quality,
            int errorCode,
            OpcItemState[] items)
        {
            TransactionId = transactionId;
            GroupHandle = groupHandle;
            MasterQuality = quality;
            MasterErrorCode = errorCode;
            Items = items;
        }
        public int TransactionId { get; private set; }
        public int GroupHandle { get; set; }
        public int MasterQuality { get; set; }
        public int MasterErrorCode { get; set; }
        public OpcItemState[] Items { get; private set; }
    }
}
