using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.Rcw
{
    public enum OpcBrowseType
    {
        Branch = OpcRcw.Da.OPCBROWSETYPE.OPC_BRANCH,
        Leaf = OpcRcw.Da.OPCBROWSETYPE.OPC_LEAF,
        Flat = OpcRcw.Da.OPCBROWSETYPE.OPC_FLAT
    }
}
