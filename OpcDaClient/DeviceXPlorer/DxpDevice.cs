namespace OpcDaClient.DeviceXPlorer
{
    public class DxpBitDevice : DxpDevice
    {
        public DxpBitDevice(string name, AddressNotation notation, string description)
        {
            Name = name;
            AddressNotation = notation;
            Description = description;
            DeviceType = DxpDeviceType.Bit;
        }
    }

    public class DxpDevice
    {
        public static readonly DxpDevice Unknown = new DxpDevice();

        internal DxpDevice()
        {
        }

        public AddressNotation AddressNotation { get; internal set; }
        public bool CanByteAccess { get; internal set; }
        public string Description { get; internal set; }
        public DxpDeviceType DeviceType { get; internal set; }
        public string Name { get; internal set; }
        public bool UseBlockNumber { get; internal set; }
    }

    public class DxpWordDevice : DxpDevice
    {
        public DxpWordDevice(string name, AddressNotation notation, string description)
        {
            Name = name;
            AddressNotation = notation;
            Description = description;
            DeviceType = DxpDeviceType.Word;
        }
    }

    public class DxpWordDeviceWithBlockNumber : DxpDevice
    {
        public DxpWordDeviceWithBlockNumber(string name, AddressNotation notation, string description)
        {
            Name = name;
            AddressNotation = notation;
            Description = description;
            UseBlockNumber = true;
        }
    }
}