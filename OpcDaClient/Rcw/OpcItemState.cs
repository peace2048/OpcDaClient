using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.Rcw
{
    public class OpcItemState
    {
        public int ErrorCode { get; set; }
        public DateTime Timestamp { get; set; }
        public int ClientHandle { get; set; }
        public object DataValue { get; set; }
        public short Quality { get; set; }
    }
}
