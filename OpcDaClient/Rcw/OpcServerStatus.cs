using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.Rcw
{
    public class OpcServerStatus
    {
        public int BandWidth { get; internal set; }
        public int GroupCount { get; internal set; }
        public OpcServerState ServerState { get; internal set; }
        public DateTime CurrentTime { get; internal set; }
        public DateTime LastUpdateTime { get; internal set; }
        public DateTime StartTime { get; internal set; }
        public string VendorInfo { get; internal set; }
        public short BuildNumber { get; internal set; }
        public short MajorVersion { get; internal set; }
        public short MinorVersion { get; internal set; }
    }
}
