using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.Rcw
{
    public interface IOpcGroupAsync : IOpcGroup
    {
        bool IsAdvise { get; set; }

        event EventHandler<CancelCompleteEventArgs> CancelComplete;
        event EventHandler<DataChangeEventArgs> DataChange;
        event EventHandler<ReadCompleteEventArgs> ReadComplete;
        event EventHandler<WriteCompleteEventArgs> WriteComplete;

        void AsyncCancel(int transactionId);
        int[] AsyncRead(int connection, OpcDataSource source, int[] serverHandles, out int transactionId);
        int[] AsyncWrite(int connection, int[] serverHandles, object[] values, out int transactionId);
        void Refresh(int connection, OpcDataSource source, out int transactionId);
    }
}
