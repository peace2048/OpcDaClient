using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.Rcw
{
    public class UserIdentity : Opc.Ua.Com.UserIdentity
    {
        public UserIdentity(string userName, string password):base(userName, password) { }
    }
}
