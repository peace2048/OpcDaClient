using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.RcwWrapper
{
    public interface IOpcGroup : IDisposable
    {
        void AddItems(OpcItemDefine[] items);
        DaValue[] Read(int[] serverHandles);
        IDisposable Watch(IOpcDataCallback callback);
    }
}
