using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpcRcw.Comn;
using Opc.Ua.Com.Client;
using Opc.Ua.Com;
using System.Runtime.InteropServices;
using System.Collections;

namespace OpcDaClient.Rcw
{
    public class OpcServerFactory : IDisposable
    {
        private static readonly Guid OpcServerListGuid = new Guid("13486D51-4821-11D2-A494-3CB306C10000");

        public static OpcServerFactory Connect()
        {
            var obj = ComUtils.CreateInstance(OpcServerListGuid, null, null);
            if (obj != null)
            {
                return new OpcServerFactory(obj);
            }
            throw new Exception(); //TODO: xxxx
        }

        private OpcServerFactory(object comInstance)
        {
            _server = new OpcServerListImpl { Unknown = comInstance };
        }


        public void Dispose()
        {
            _server.Dispose();
        }

        public OpcServer CreateFromProgId(string progId)
        {
            var clsid = Guid.Empty;
            _server.CLSIDFromProgID(progId, out clsid);
            return CreateFromClsId(clsid);
        }

        public string GetProgIdFromClsId(Guid clsid)
        {
            string ppszProgID = null;
            string ppszUserType = null;
            string ppszVerIndProgID = null;
            _server.GetClassDetails(ref clsid, out ppszProgID, out ppszUserType, out ppszVerIndProgID);
            if (!string.IsNullOrEmpty(ppszVerIndProgID))
            {
                return ppszVerIndProgID;
            }
            return ppszProgID;
        }

        public OpcServer CreateFromClsId(Guid clsid)
        {
            var obj = ComUtils.CreateInstance(clsid, null, null);
            if (obj != null)
            {
                return new OpcServer(obj);
            }
            return null;
        }

        public List<Guid> GetClsIds()
        {
            var categories = new[]
            {
                new Guid("63D5F432-CFE4-11d1-B2C8-0060083BA1FB"), // DA 2.xxx
                new Guid("CC603642-66D7-48f1-B69A-B625E73652D7"), // DA 3.0
            };
            IOPCEnumGUID enumerator = null;
            _server.EnumClassesOfCategories(2, categories, 0, null, out enumerator);
            var guids = new List<Guid>();
            try
            {
                var buffer = new Uri[10];
                var guidSize = Marshal.SizeOf<Guid>();
                int fetched = 0;
                do
                {
                    var ptr = Marshal.AllocCoTaskMem(guidSize * buffer.Length);
                    try
                    {
                        enumerator.Next(buffer.Length, ptr, out fetched);
                        if (fetched > 0)
                        {
                            var p = ptr;
                            for (int i = 0; i < fetched; i++, p += guidSize)
                            {
                                var v = Marshal.PtrToStructure<Guid>(p);
                                Marshal.DestroyStructure<Guid>(p);
                                guids.Add(v);
                            }
                        }
                    }
                    finally
                    {
                        Marshal.FreeCoTaskMem(ptr);
                    }
                }
                while (fetched > 0);
            }
            catch { }
            finally
            {
                ComUtils.ReleaseServer(enumerator);
            }
            return guids;
        }

        private OpcServerListImpl _server;

        private class OpcServerListImpl : ComObject, IOPCServerList2
        {
            public void CLSIDFromProgID(string szProgId, out Guid clsid)
            {
                var methodName = nameof(IOPCServerList2) + "." + nameof(IOPCServerList2.CLSIDFromProgID);
                try
                {
                    var server = BeginComCall<IOPCServerList2>(methodName, true);
                    server.CLSIDFromProgID(szProgId, out clsid);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            public void EnumClassesOfCategories(int cImplemented, Guid[] rgcatidImpl, int cRequired, Guid[] rgcatidReq, out IOPCEnumGUID ppenumClsid)
            {
                var methodName = nameof(IOPCServerList2) + "." + nameof(IOPCServerList2.EnumClassesOfCategories);
                try
                {
                    var server = BeginComCall<IOPCServerList2>(methodName, true);
                    server.EnumClassesOfCategories(cImplemented, rgcatidImpl, cRequired, rgcatidReq, out ppenumClsid);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            public void GetClassDetails(ref Guid clsid, out string ppszProgID, out string ppszUserType, out string ppszVerIndProgID)
            {
                var methodName = nameof(IOPCServerList2) + "." + nameof(IOPCServerList2.GetClassDetails);
                try
                {
                    var server = BeginComCall<IOPCServerList2>(methodName, true);
                    server.GetClassDetails(clsid, out ppszProgID, out ppszUserType, out ppszVerIndProgID);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }
        }
    }
}
