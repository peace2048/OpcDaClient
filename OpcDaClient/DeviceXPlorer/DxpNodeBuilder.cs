namespace OpcDaClient.DeviceXPlorer
{
    public class DxpNodeBuilder : DxpNodeBuilderBase
    {
        public DxpNodeBuilder Skip(int n)
        {
            Address += n;
            return this;
        }

        public DxpItemInt16 ToItemInt16() => new DxpItemInt16(ToNode());
        public DxpItemInt16 ToItemInt16(short defaultValue) => new DxpItemInt16(ToNode()) { Value = defaultValue };

        public DxpItemInt16Array ToItemInt16Array(int deviceCount) => new DxpItemInt16Array(ToNode(deviceCount));
        public DxpItemInt16Array ToItemInt16Array(short[] defaultValue) => new DxpItemInt16Array(ToNode(defaultValue.Length)) { Value = defaultValue };

        public DxpItemString ToItemString(int deviceCount) => new DxpItemString(ToNode(deviceCount));
        public DxpItemString ToItemString(int deviceCount, string defaultValue) => new DxpItemString(ToNode(deviceCount)) { Value = defaultValue };
    }

    public class DxpNodeBuilderBase
    {
        public static string DefaultAccessPath { get; set; } = "Device1";

        public static DxpNodeBuilder Create(DxpWordDevice device, int address, string accessPath = null)
            => new DxpNodeBuilder { AccessPath = accessPath ?? DefaultAccessPath, Address = address, Device = device };

        public static DxpNodeBuilderBitDevice Create(DxpBitDevice device, int address, string accessPath = null)
            => new DxpNodeBuilderBitDevice { AccessPath = accessPath ?? DefaultAccessPath, Address = address, Device = device };

        public static DxpNodeBuilderWithBlockNumber Create(DxpWordDeviceWithBlockNumber device, int block, int address, string accessPath = null)
            => new DxpNodeBuilderWithBlockNumber { AccessPath = accessPath ?? DefaultAccessPath, Address = address, BlockNumber = block, Device = device };

        public string AccessPath { get; set; } = DefaultAccessPath;
        public int Address { get; set; }
        public bool AutoIncrement { get; set; } = true;
        public DxpDevice Device { get; set; }

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

    public class DxpNodeBuilderBitDevice : DxpNodeBuilderBase
    {
        public DxpNodeBuilderBitDevice Skip(int n)
        {
            Address += n;
            return this;
        }

        public DxpItemBoolean ToItemBoolean() => new DxpItemBoolean(ToNode());

        public DxpItemBooleanArray ToItemBooleanArray(int deviceCount) => new DxpItemBooleanArray(ToNode(deviceCount));

        public DxpItemInt16 ToItemInt16() => new DxpItemInt16(ToNode());

        public DxpItemInt16Array ToItemInt16Array(int deviceCount) => new DxpItemInt16Array(ToNode(deviceCount));
    }

    public class DxpNodeBuilderWithBlockNumber : DxpNodeBuilderBase
    {
        public int BlockNumber { get; set; }

        public DxpNodeBuilderWithBlockNumber Skip(int n)
        {
            Address += n;
            return this;
        }

        public DxpItemInt16 ToItemInt16() => new DxpItemInt16(ToNode());

        public DxpItemInt16Array ToItemInt16Array(int deviceCount) => new DxpItemInt16Array(ToNode(deviceCount));
    }
}