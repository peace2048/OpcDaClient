using System;
using System.Text;

namespace OpcDaClient.DeviceXPlorer
{
    public class DxpNode
    {
        private string _accessPath;
        private int _address;
        private DxpDevice _device;
        private int _extNumber;
        private string _itemId;
        private int _size;

        public string AccessPath
        {
            get { return _accessPath; }
            set { _accessPath = value; BuildItemId(); }
        }

        public int Address
        {
            get { return _address; }
            set { _address = value; BuildItemId(); }
        }

        public DxpDevice Device
        {
            get { return _device; }
            set { _device = value; BuildItemId(); }
        }

        public int ExtNumber
        {
            get { return _extNumber; }
            set { _extNumber = value; BuildItemId(); }
        }

        public string ItemId
        {
            get { return _itemId; }
            set { _itemId = value; Parse(); }
        }

        public int Size
        {
            get { return _size; }
            set { _size = value; BuildItemId(); }
        }

        public override string ToString()
        {
            return ItemId ?? base.ToString();
        }

        protected virtual void BuildItemId()
        {
            if (Device == null || Device.DeviceType == DxpDeviceType.Unknown)
            {
                return;
            }
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(AccessPath))
            {
                sb.Append(AccessPath).Append(".");
            }
            sb.Append(Device.Name);
            if (Device.UseBlockNumber)
            {
                sb.Append(ExtNumber).Append(':');
            }
            switch (Device.AddressNotation)
            {
                case AddressNotation.Octal:
                    sb.Append(Convert.ToString(Address, 8));
                    break;

                case AddressNotation.Hexadecimal:
                    sb.Append(Address.ToString("X"));
                    break;

                default:
                    sb.Append(Address);
                    break;
            }
            if (Size > 1)
            {
                sb.Append(":A").Append(Size);
            }
            _itemId = sb.ToString();
        }

        protected virtual void Parse()
        {
            AccessPath = string.Empty;
            Address = 0;
            Device = DxpDevice.Unknown;
            Size = 0;
        }
    }
}