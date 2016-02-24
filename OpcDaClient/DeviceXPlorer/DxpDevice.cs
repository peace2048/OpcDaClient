namespace OpcDaClient.DeviceXPlorer
{
    public class DxpDevice
    {
        public static readonly DxpDevice Unknown = new DxpDevice(string.Empty, string.Empty, DxpDeviceType.Unknown, AddressNotation.Unknown);
        public static readonly DxpDevice UnknownBitDevice = new DxpDevice(string.Empty, string.Empty, DxpDeviceType.Bit, AddressNotation.Unknown);
        public static readonly DxpDevice UnknownByteDevice = new DxpDevice(string.Empty, string.Empty, DxpDeviceType.Byte, AddressNotation.Unknown);
        public static readonly DxpDevice UnknownWordDevice = new DxpDevice(string.Empty, string.Empty, DxpDeviceType.Word, AddressNotation.Unknown);

        internal DxpDevice(string name, string description, DxpDeviceType deviceType, AddressNotation notation)
        {
            Name = name;
            Description = description;
            DeviceType = deviceType;
            AddressNotation = notation;
        }

        private DxpDevice()
        {
        }

        public AddressNotation AddressNotation { get; private set; }
        public string Description { get; private set; }
        public DxpDeviceType DeviceType { get; private set; }
        public string Name { get; private set; }
    }
}