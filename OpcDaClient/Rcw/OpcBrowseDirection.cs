using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.Rcw
{
    public enum OpcBrowseDirection
    {
        Up = OpcRcw.Da.OPCBROWSEDIRECTION.OPC_BROWSE_UP,
        Down = OpcRcw.Da.OPCBROWSEDIRECTION.OPC_BROWSE_DOWN,
        To = OpcRcw.Da.OPCBROWSEDIRECTION.OPC_BROWSE_TO
    }
}
