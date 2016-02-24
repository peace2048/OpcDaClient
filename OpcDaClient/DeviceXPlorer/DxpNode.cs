using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.DeviceXPlorer
{
    public class DxpNode
    {
        public virtual string ItemId { get; set; }
        public DxpDeviceType DeviceType { get; set; }
        public int Address { get; set; }
        public int Size { get; set; }
    }
}
