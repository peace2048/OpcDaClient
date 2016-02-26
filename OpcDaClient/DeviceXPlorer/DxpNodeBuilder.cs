using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.DeviceXPlorer
{
    public class DxpNodeBuilderBase
    {
        public DxpDevice Device { get; set; }
        public int Address { get; set; }
        public bool AutoIncrement { get; set; } = true;

        public DxpNode ToNode()
        {
            var node =   new DxpNode { Address = Address, Device = Device };
            if (AutoIncrement)
            {
                Address++;
            }
            return node;
        }

        public DxpNode ToNode(int deviceCount)
        {
            var node = new DxpNode { Address = Address, Device = Device, Size = deviceCount };
            if (AutoIncrement)
            {
                Address += deviceCount;
            }
            return node;
        }
    }
    public class DxpNodeBuilder : DxpNodeBuilderBase
    {
        public DxpItemInt16 ToItemInt16() => new DxpItemInt16(ToNode());
        public DxpItemInt16Array ToItemInt16Array(int deviceCount) => new DxpItemInt16Array(ToNode(deviceCount));

        public DxpNodeBuilder Skip(int n)
        {
            Address += n;
            return this;
        }
    }

    public class DxpNodeBuilderWithBlockNumber : DxpNodeBuilderBase
    {
        public int BlockNumber { get; set; }

        public DxpItemInt16 ToItemInt16() => new DxpItemInt16(ToNode());
        public DxpItemInt16Array ToItemInt16Array(int deviceCount) => new DxpItemInt16Array(ToNode(deviceCount));

        public DxpNodeBuilderWithBlockNumber Skip(int n)
        {
            Address += n;
            return this;
        }
    }

    public class DxpNodeBuilderBitDevice : DxpNodeBuilderBase
    {
        public DxpItemBoolean ToItemBoolean() => new DxpItemBoolean(ToNode());
        public DxpItemBooleanArray ToItemBooleanArray(int deviceCount) => new DxpItemBooleanArray(ToNode(deviceCount));
        public DxpItemInt16 ToItemInt16() => new DxpItemInt16(ToNode());
        public DxpItemInt16Array ToItemInt16Array(int deviceCount) => new DxpItemInt16Array(ToNode(deviceCount));

        public DxpNodeBuilderBitDevice Skip(int n)
        {
            Address += n;
            return this;
        }
    }
}
