using OpcRcw.Da;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.Rcw
{
    public enum OpcEnumScope
    {
        All = OPCENUMSCOPE.OPC_ENUM_ALL,
        AllConnections = OPCENUMSCOPE.OPC_ENUM_ALL_CONNECTIONS,
        Private = OPCENUMSCOPE.OPC_ENUM_PRIVATE,
        PrivateConnections = OPCENUMSCOPE.OPC_ENUM_PRIVATE_CONNECTIONS,
        Public = OPCENUMSCOPE.OPC_ENUM_PUBLIC,
        PublicConnections = OPCENUMSCOPE.OPC_ENUM_PUBLIC_CONNECTIONS
    }
}
