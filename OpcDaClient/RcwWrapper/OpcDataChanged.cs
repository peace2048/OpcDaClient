using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.RcwWrapper
{
    public class OpcDataChanged
    {
        public int ClientHandle { get; internal set; }
        public int ErrorCode { get; internal set; }
        public short Quality { get; internal set; }
        public DateTime Timestamp { get; internal set; }
        public object Value { get; internal set; }
    }
}
