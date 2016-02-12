using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.Melsec
{
    public class MelNodeSequenceGenerator
    {
        public MelNodeSequenceGenerator(MelDevice device, int startAddress)
        {
            Device = device;
            NextAddress = startAddress;
        }

        public MelDevice Device { get; set; }
        public int NextAddress { get; set; }

        public MelNode CreateNode(int length)
        {
            var r = new MelNode { Address = NextAddress, Device = Device, Length = length };
            NextAddress += length;
            return r;
        }
        public DaItem<T> CreateDaItem<T>(int length)
        {
            return new DaItem<T> { Node = CreateNode(length) };
        }

    }
}
