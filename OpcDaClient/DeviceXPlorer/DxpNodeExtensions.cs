using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.DeviceXPlorer
{
    public static class DxpNodeExtensions
    {
        public static DxpNodeBuilder Address(this DxpDevice device, int address) => new DxpNodeBuilder { Address = address, Device = device };
        public static DxpNodeBuilderBitDevice Address(this DxpBitDevice device, int address) => new DxpNodeBuilderBitDevice { Address = address, Device = device };
        public static DxpNodeBuilderWithBlockNumber Address(this DxpWordDeviceWithBlockNumber device, int block, int address) => new DxpNodeBuilderWithBlockNumber { Address = address, Device = device, BlockNumber = block };
    }
}
