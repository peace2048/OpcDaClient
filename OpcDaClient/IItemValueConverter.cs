using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient
{
    public interface IItemValueConverter<T>
    {
        T FromDaItem(DaItem item);
        object ToObject(T value);
    }
}
