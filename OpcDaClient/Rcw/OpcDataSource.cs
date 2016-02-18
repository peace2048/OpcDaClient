using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.Rcw
{
    public enum OpcDataSource
    {
        Cache = OpcRcw.Da.OPCDATASOURCE.OPC_DS_CACHE,
        Device = OpcRcw.Da.OPCDATASOURCE.OPC_DS_DEVICE
    }
}
