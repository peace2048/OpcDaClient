using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.Melsec
{
    public class MelNode : DaNode
    {
        public MelDevice Device { get; set; }
        public int Address { get; set; }
        public int Length { get; set; }
        public override string ItemId
        {
            get
            {
                return "Device1." + Device.Name + Address.ToString();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public MelNode Next(int length)
        {
            return new MelNode { Address = Address + Length, Device = Device, Length = length };
        }
    }

    public class MelDevice
    {
        public static readonly MelDevice D = new MelDevice { Name = "D" };

        private MelDevice() { }
        public string Name { get; private set; }
    }
}
