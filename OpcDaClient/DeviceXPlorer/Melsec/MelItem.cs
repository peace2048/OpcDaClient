using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.DeviceXPlorer.Melsec
{
    public class MelItem<T> : DxpItem<T>
    {
        public MelItem(string itemId)
        {
            this.Node = new MelNode(itemId);
        }
    }
}
