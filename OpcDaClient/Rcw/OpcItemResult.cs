using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.Rcw
{
    public class OpcItemResult
    {
        public int ErrorCode { get; set; }
        public int AccessRights { get; set; }
        public int ServerHandle { get; set; }
        public short CanonicalDataType { get; set; }
    }
}
