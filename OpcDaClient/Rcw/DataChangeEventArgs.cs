using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.Rcw
{
    public class DataChangeEventArgs : EventArgs
    {
        public DataChangeEventArgs(
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
        public int GroupHandle { get; private set; }
        public int MasterQuality { get; private set; }
        public int MasterErrorCode { get; private set; }
        public OpcItemState[] Items { get; private set; }
    }
}
