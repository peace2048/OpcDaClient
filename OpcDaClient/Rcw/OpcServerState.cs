using OpcRcw.Da;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.Rcw
{
    public enum OpcServerState
    {
        CommFault = OPCSERVERSTATE.OPC_STATUS_COMM_FAULT,
        Faild = OPCSERVERSTATE.OPC_STATUS_FAILED,
        NoConfig = OPCSERVERSTATE.OPC_STATUS_NOCONFIG,
        Running = OPCSERVERSTATE.OPC_STATUS_RUNNING,
        Suspended = OPCSERVERSTATE.OPC_STATUS_SUSPENDED,
        Test = OPCSERVERSTATE.OPC_STATUS_TEST
    }
}
