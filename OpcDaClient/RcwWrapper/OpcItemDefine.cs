using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.RcwWrapper
{
    public class OpcItemDefine
    {
        public bool IsActive { get; set; }
        public int ClientHandle { get; set; }
        public string ItemId { get; set; }
        public int ServerHandle { get; internal set; }
        public int ErrorCode { get; internal set; }
        public VarEnum DataType { get; internal set; }
    }
}
