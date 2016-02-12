using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpcDaClient.RcwWrapper
{
    class OpcServer : Opc.Ua.Com.Client.ComObject, IOpcServer
    {
        public OpcServer(object obj)
        {
            Unknown = obj;
        }

        public IOpcGroup AddGroup(string name, bool isActive, int updateRate, float deadband, int localeId)
        {
            int serverHandle;
            GCHandle hDeadband = GCHandle.Alloc(deadband, GCHandleType.Pinned);
            int revisedUpdateRate;
            var iid = typeof(OpcRcw.Da.IOPCItemMgt).GUID;
            object unknown;
            const string methodName = "IOPCServer.AddGroup";
            try
            {
                var server = BeginComCall<OpcRcw.Da.IOPCServer>(methodName, true);
                server.AddGroup(name, isActive ? 1 : 0, updateRate, 0, IntPtr.Zero, hDeadband.AddrOfPinnedObject(), localeId, out serverHandle, out revisedUpdateRate, ref iid, out unknown);
                return new RcwWrapper.OpcGroup(unknown);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                EndComCall(methodName);
                hDeadband.Free();
            }
        }
    }
}
