using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient
{
    public interface IDaItem
    {
        string ItemId { get; }
        DaValue Result { get; }
    }
}
