using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.DeviceXPlorer
{
    public class DxpItem : IDaItem
    {
        public string ItemId { get { return Node.ItemId; } }

        public DxpNode Node { get; set; }

        public DaValue Result { get; private set; } = new DaValue();
    }

    public class DxpItem<T> : DxpItem
    {

    }
}
