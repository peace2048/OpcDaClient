using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.DeviceXPlorer.Melsec
{
    public class MelNodeSequenceGenerator
    {
        public MelNodeSequenceGenerator(DxpDevice device, int startAddress)
        {
            Device = device;
            NextAddress = startAddress;
        }

        public DxpDevice Device { get; set; }
        public int NextAddress { get; set; }

        public DxpNode CreateNode(int length)
        {
            var r = new DxpNode { Address = NextAddress, Device = Device, Size = length };
            NextAddress += length;
            return r;
        }
        public DxpItem<T> CreateDaItem<T>(int length)
        {
            return new DxpItem<T> { Node = CreateNode(length) };
        }

    }
}
