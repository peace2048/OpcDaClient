using Opc.Ua.Com;
using Opc.Ua.Com.Client;
using OpcRcw.Comn;
using OpcRcw.Da;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace OpcDaClient.Rcw
{
    public class OpcServer : IDisposable
    {
        private OpcServerImpl _server;

        internal OpcServer(object comInstance)
        {
            _server = new OpcServerImpl { Unknown = comInstance };
        }

        public OpcGroup AddGroup(string name, bool isActive, int requestedUpdateRate, int clientHandle, float deadband, int localeId)
        {
            var hDeadband = GCHandle.Alloc(deadband, GCHandleType.Pinned);
            try
            {
                var timeBias = IntPtr.Zero;
                var serverHandle = 0;
                var revisedUpdateRate = 0;
                var iid = typeof(IOPCItemMgt).GUID;
                object ppUnk = null;
                _server.AddGroup(
                    name,
                    isActive ? 1 : 0,
                    requestedUpdateRate,
                    clientHandle,
                    timeBias,
                    hDeadband.AddrOfPinnedObject(),
                    localeId,
                    out serverHandle,
                    out revisedUpdateRate,
                    ref iid,
                    out ppUnk);
                return new OpcGroup(ppUnk, serverHandle, revisedUpdateRate);
            }
            finally
            {
                hDeadband.Free();
            }
        }

        public void Dispose()
        {
            _server.Dispose();
        }

        public string GetErrorString(int errorCode)
        {
            string ppString = null;
            _server.GetErrorString(errorCode, out ppString);
            return ppString;
        }

        public string GetErrorString(int errorCode, int localeId)
        {
            string ppString = null;
            _server.GetErrorString(errorCode, localeId, out ppString);
            return ppString;
        }

        public OpcGroup GetGroupByName(string name)
        {
            var iid = typeof(IOPCItemMgt).GUID;
            object ppUnk = null;
            _server.GetGroupByName(name, ref iid, out ppUnk);
            return new OpcGroup(ppUnk);
        }

        public int GetLocaleId()
        {
            int pdwLcid = 0;
            _server.GetLocaleID(out pdwLcid);
            return pdwLcid;
        }

        public OpcServerStatus GetStatus()
        {
            var ppStatus = IntPtr.Zero;
            _server.GetStatus(out ppStatus);
            var status = Marshal.PtrToStructure<OPCSERVERSTATUS>(ppStatus);
            Marshal.DestroyStructure<OPCSERVERSTATUS>(ppStatus);
            Marshal.FreeCoTaskMem(ppStatus);
            return new OpcServerStatus
            {
                BandWidth = status.dwBandWidth,
                BuildNumber = status.wBuildNumber,
                CurrentTime = ComUtils.GetDateTime(status.ftCurrentTime),
                GroupCount = status.dwGroupCount,
                LastUpdateTime = ComUtils.GetDateTime(status.ftLastUpdateTime),
                MajorVersion = status.wMajorVersion,
                MinorVersion = status.wMinorVersion,
                ServerState = (OpcServerState)status.dwServerState,
                StartTime = ComUtils.GetDateTime(status.ftStartTime),
                VendorInfo = status.szVendorInfo
            };
        }

        public int[] QueryAvailableLocaleIDs()
        {
            var pdwCount = 0;
            var pdwLcid = IntPtr.Zero;
            _server.QueryAvailableLocaleIDs(out pdwCount, out pdwLcid);
            return ComUtils.GetInt32s(ref pdwLcid, pdwCount, true);
        }

        public void QueryAvailableProperties(string itemId)
        {
            var pdwCount = 0;
            var ppPropertyIds = IntPtr.Zero;
            var ppDescriptions = IntPtr.Zero;
            var ppvtDataTypes = IntPtr.Zero;
            _server.QueryAvailableProperties(itemId, out pdwCount, out ppPropertyIds, out ppDescriptions, out ppvtDataTypes);
            if (pdwCount > 0)
            {
                var ids = ComUtils.GetInt32s(ref ppPropertyIds, pdwCount, true);
                var descs = ComUtils.GetUnicodeStrings(ref ppDescriptions, pdwCount, true);
                var types = ComUtils.GetInt16s(ref ppvtDataTypes, pdwCount, true);
            }
        }

        public void RemoveGroup(int serverHandle, bool force)
        {
            _server.RemoveGroup(serverHandle, force ? 1 : 0);
        }

        public string[] BrowseAccessPaths(string itemId)
        {
            IEnumString obj = null;
            _server.BrowseAccessPaths(itemId, out obj);
            var enumString = new EnumString(obj, 100);
            return Enumerable.Repeat(enumString, int.MaxValue).Select(_ => _.Next()).TakeWhile(_ => _ != null).ToArray();
        }

        public string[] BrowseItemIds(OpcBrowseType browseType, string filterCriteria, short dataTypeFilter, int accessRightsFilter)
        {
            IEnumString ppIEnumString = null;
            _server.BrowseOPCItemIDs((OPCBROWSETYPE)browseType, filterCriteria, dataTypeFilter, accessRightsFilter, out ppIEnumString);
            var enumString = new EnumString(ppIEnumString, 100);
            return Enumerable.Repeat(enumString, int.MaxValue).Select(_ => _.Next()).TakeWhile(_ => _ != null).ToArray();
        }

        public void ChangeBrowsePosition(OpcBrowseDirection direction, string name)
        {
            _server.ChangeBrowsePosition((OPCBROWSEDIRECTION)direction, name);
        }

        public string GetItemId(string itemDataId)
        {
            string itemId = null;
            _server.GetItemID(itemDataId, out itemId);
            return itemId;
        }

        public void SetClientName(string name)
        {
            _server.SetClientName(name);
        }

        public void SetLocalId(int localeId)
        {
            _server.SetLocaleID(localeId);
        }

        private class OpcServerImpl : ComObject, IOPCServer, IOPCCommon, IOPCItemProperties, IOPCBrowseServerAddressSpace
        {
            public void AddGroup(string szName, int bActive, int dwRequestedUpdateRate, int hClientGroup, IntPtr pTimeBias, IntPtr pPercentDeadband, int dwLCID, out int phServerGroup, out int pRevisedUpdateRate, ref Guid riid, out object ppUnk)
            {
                var methodName = nameof(IOPCServer) + "." + nameof(IOPCServer.AddGroup);
                try
                {
                    var server = BeginComCall<IOPCServer>(methodName, true);
                    server.AddGroup(szName, bActive, dwRequestedUpdateRate, hClientGroup, pTimeBias, pPercentDeadband, dwLCID, out phServerGroup, out pRevisedUpdateRate, riid, out ppUnk);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            public void CreateGroupEnumerator(OPCENUMSCOPE dwScope, ref Guid riid, out object ppUnk)
            {
                var methodName = nameof(IOPCServer) + "." + nameof(IOPCServer.CreateGroupEnumerator);
                try
                {
                    var server = BeginComCall<IOPCServer>(methodName, true);
                    server.CreateGroupEnumerator(dwScope, ref riid, out ppUnk);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            public void GetErrorString(int dwError, out string ppString)
            {
                var methodName = nameof(IOPCCommon) + "." + nameof(IOPCCommon.GetErrorString);
                try
                {
                    var server = BeginComCall<IOPCCommon>(methodName, true);
                    server.GetErrorString(dwError, out ppString);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            public void GetErrorString(int dwError, int dwLocale, out string ppString)
            {
                var methodName = nameof(IOPCServer) + "." + nameof(IOPCServer.GetErrorString);
                try
                {
                    var server = BeginComCall<IOPCServer>(methodName, true);
                    server.GetErrorString(dwError, dwLocale, out ppString);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            public void GetGroupByName(string szName, ref Guid riid, out object ppUnk)
            {
                var methodName = nameof(IOPCServer) + "." + nameof(IOPCServer.GetGroupByName);
                try
                {
                    var server = BeginComCall<IOPCServer>(methodName, true);
                    server.GetGroupByName(szName, ref riid, out ppUnk);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            public void GetItemProperties(string szItemID, int dwCount, int[] pdwPropertyIDs, out IntPtr ppvData, out IntPtr ppErrors)
            {
                var methodName = nameof(IOPCItemProperties) + "." + nameof(IOPCItemProperties.GetItemProperties);
                try
                {
                    var server = BeginComCall<IOPCItemProperties>(methodName, true);
                    server.GetItemProperties(szItemID, dwCount, pdwPropertyIDs, out ppvData, out ppErrors);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            public void GetLocaleID(out int pdwLcid)
            {
                var methodName = nameof(IOPCCommon) + "." + nameof(IOPCCommon.GetLocaleID);
                try
                {
                    var server = BeginComCall<IOPCCommon>(methodName, true);
                    server.GetLocaleID(out pdwLcid);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            public void GetStatus(out IntPtr ppServerStatus)
            {
                var methodName = nameof(IOPCServer) + "." + nameof(IOPCServer.GetStatus);
                try
                {
                    var server = BeginComCall<IOPCServer>(methodName, true);
                    server.GetStatus(out ppServerStatus);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            public void LookupItemIDs(string szItemID, int dwCount, int[] pdwPropertyIDs, out IntPtr ppszNewItemIDs, out IntPtr ppErrors)
            {
                var methodName = nameof(IOPCItemProperties) + "." + nameof(IOPCItemProperties.LookupItemIDs);
                try
                {
                    var server = BeginComCall<IOPCItemProperties>(methodName, true);
                    server.LookupItemIDs(szItemID, dwCount, pdwPropertyIDs, out ppszNewItemIDs, out ppErrors);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            public void QueryAvailableLocaleIDs(out int pdwCount, out IntPtr pdwLcid)
            {
                var methodName = nameof(IOPCCommon) + "." + nameof(IOPCCommon.QueryAvailableLocaleIDs);
                try
                {
                    var server = BeginComCall<IOPCCommon>(methodName, true);
                    server.QueryAvailableLocaleIDs(out pdwCount, out pdwLcid);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            public void QueryAvailableProperties(string szItemID, out int pdwCount, out IntPtr ppPropertyIDs, out IntPtr ppDescriptions, out IntPtr ppvtDataTypes)
            {
                var methodName = nameof(IOPCItemProperties) + "." + nameof(IOPCItemProperties.QueryAvailableProperties);
                try
                {
                    var server = BeginComCall<IOPCItemProperties>(methodName, true);
                    server.QueryAvailableProperties(szItemID, out pdwCount, out ppPropertyIDs, out ppDescriptions, out ppvtDataTypes);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            public void RemoveGroup(int hServerGroup, int bForce)
            {
                var methodName = nameof(IOPCServer) + "." + nameof(IOPCServer.RemoveGroup);
                try
                {
                    var server = BeginComCall<IOPCServer>(methodName, true);
                    server.RemoveGroup(hServerGroup, bForce);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            public void SetClientName(string szName)
            {
                var methodName = nameof(IOPCCommon) + "." + nameof(IOPCCommon.SetClientName);
                try
                {
                    var server = BeginComCall<IOPCCommon>(methodName, true);
                    server.SetClientName(szName);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            public void SetLocaleID(int dwLcid)
            {
                var methodName = nameof(IOPCCommon) + "." + nameof(IOPCCommon.SetLocaleID);
                try
                {
                    var server = BeginComCall<IOPCCommon>(methodName, true);
                    server.SetLocaleID(dwLcid);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            public void BrowseAccessPaths(string szItemID, out IEnumString pIEnumString)
            {
                var methodName = nameof(IOPCBrowseServerAddressSpace) + "." + nameof(IOPCBrowseServerAddressSpace.BrowseAccessPaths);
                try
                {
                    var server = BeginComCall<IOPCBrowseServerAddressSpace>(methodName, true);
                    server.BrowseAccessPaths(szItemID, out pIEnumString);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            public void BrowseOPCItemIDs(OPCBROWSETYPE dwBrowseFilterType, string szFilterCriteria, short vtDataTypeFilter, int dwAccessRightsFilter, out IEnumString ppIEnumString)
            {
                var methodName = nameof(IOPCBrowseServerAddressSpace) + "." + nameof(IOPCBrowseServerAddressSpace.BrowseOPCItemIDs);
                try
                {
                    var server = BeginComCall<IOPCBrowseServerAddressSpace>(methodName, true);
                    server.BrowseOPCItemIDs(dwBrowseFilterType, szFilterCriteria, vtDataTypeFilter, dwAccessRightsFilter, out ppIEnumString);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            public void ChangeBrowsePosition(OPCBROWSEDIRECTION dwBrowseDirection, string szString)
            {
                var methodName = nameof(IOPCBrowseServerAddressSpace) + "." + nameof(IOPCBrowseServerAddressSpace.ChangeBrowsePosition);
                try
                {
                    var server = BeginComCall<IOPCBrowseServerAddressSpace>(methodName, true);
                    server.ChangeBrowsePosition(dwBrowseDirection, szString);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            public void GetItemID(string szItemDataID, out string szItemID)
            {
                var methodName = nameof(IOPCBrowseServerAddressSpace) + "." + nameof(IOPCBrowseServerAddressSpace.GetItemID);
                try
                {
                    var server = BeginComCall<IOPCBrowseServerAddressSpace>(methodName, true);
                    server.GetItemID(szItemDataID, out szItemID);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            public void QueryOrganization(out OPCNAMESPACETYPE pNameSpaceType)
            {
                var methodName = nameof(IOPCBrowseServerAddressSpace) + "." + nameof(IOPCBrowseServerAddressSpace.QueryOrganization);
                try
                {
                    var server = BeginComCall<IOPCBrowseServerAddressSpace>(methodName, true);
                    server.QueryOrganization(out pNameSpaceType);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }
        }
    }
}