using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient
{
    class DefaultItemValueConverter<T> : IItemValueConverter<T>
    {
        public T FromDaItem(DaItem item)
        {
            if (item.RawValue == null)
            {
                return default(T);
            }
            return (T)item.RawValue;
        }

        public object ToObject(T value)
        {
            return value;
        }
    }
}
