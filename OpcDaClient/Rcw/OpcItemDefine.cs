using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.Rcw
{
    public class OpcItemDefine
    {
        public bool IsActive { get; set; }
        public int ClientHandle { get; set; }
        public string AccessPath { get; set; }
        public string ItemId { get; set; }
        public short RequestedDataType { get; set; }
    }
}
