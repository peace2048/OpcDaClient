using Opc.Ua.Com;
using Opc.Ua.Com.Client;
using OpcRcw.Da;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace OpcDaClient.Rcw
{
    public class OpcGroup : IOpcGroupAsync
    {
        private OPCDataCallbackImpl _callback;
        private ConnectionPoint _connectionPoint;
        private OpcGroupImpl _impl;

        public OpcGroup(object unknown)
        {
            _callback = new OPCDataCallbackImpl(this);
            _impl = new OpcGroupImpl(unknown);
        }

        public OpcGroup(object unknown, int serverHandle, int updateRate)
            : this(unknown)
        {
        }

        public event EventHandler<CancelCompleteEventArgs> CancelComplete;

        public event EventHandler<DataChangeEventArgs> DataChange;

        public event EventHandler<ReadCompleteEventArgs> ReadComplete;

        public event EventHandler<WriteCompleteEventArgs> WriteComplete;

        public bool IsAdvise
        {
            get
            {
                lock (_impl.Lock)
                {
                    return _connectionPoint != null;
                }
            }
            set
            {
                lock (_impl.Lock)
                {
                    if (value)
                    {
                        if (_connectionPoint != null)
                        {
                            _connectionPoint = new ConnectionPoint(_impl.Unknown, typeof(IOPCDataCallback).GUID);
                            _connectionPoint.Advise(_callback);
                        }
                    }
                    else
                    {
                        if (_connectionPoint != null)
                        {
                            _connectionPoint.Unadvise();
                            _connectionPoint.Dispose();
                            _connectionPoint = null;
                        }
                    }
                }
            }
        }

        private IOPCAsyncIO AsyncIO { get { return _impl; } }
        private IOPCGroupStateMgt GroupStateMgt { get { return _impl; } }
        private IOPCItemMgt ItemMgt { get { return _impl; } }
        private IOPCSyncIO SyncIO { get { return _impl; } }

        public OpcItemResult[] AddItems(OpcItemDefine[] items)
        {
            var rawItems = items.Select(item => new OPCITEMDEF
            {
                bActive = item.IsActive ? 1 : 0,
                hClient = item.ClientHandle,
                szAccessPath = item.AccessPath,
                szItemID = item.ItemId,
                vtRequestedDataType = item.RequestedDataType
            })
            .ToArray();

            var ppAddResults = IntPtr.Zero;
            var ppErrors = IntPtr.Zero;

            ItemMgt.AddItems(items.Length, rawItems, out ppAddResults, out ppErrors);

            var errors = ComUtils.GetInt32s(ref ppErrors, items.Length, true);
            var results = GetStructures<OPCITEMRESULT>(ppAddResults, items.Length).Zip(errors, (r, e) =>
            {
                try
                {
                    if (r.pBlob != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(r.pBlob);
                        r.pBlob = IntPtr.Zero;
                    }
                    return new OpcItemResult
                    {
                        AccessRights = r.dwAccessRights,
                        CanonicalDataType = r.vtCanonicalDataType,
                        ErrorCode = e,
                        ServerHandle = r.hServer
                    };
                }
                catch (Exception ex)
                {
                    return new OpcItemResult { ErrorCode = ex.HResult };
                }
            })
            .ToArray();

            return results;
        }

        public void AsyncCancel(int transactionId)
        {
            AsyncIO.Cancel(transactionId);
        }

        public int[] AsyncRead(int connection, OpcDataSource source, int[] serverHandles, out int transactionId)
        {
            var ppErrors = IntPtr.Zero;
            AsyncIO.Read(connection, (OPCDATASOURCE)source, serverHandles.Length, serverHandles, out transactionId, out ppErrors);
            return ComUtils.GetInt32s(ref ppErrors, serverHandles.Length, true);
        }

        public int[] AsyncWrite(int connection, int[] serverHandles, object[] values, out int transactionId)
        {
            var ppErrors = IntPtr.Zero;
            AsyncIO.Write(connection, serverHandles.Length, serverHandles, values, out transactionId, out ppErrors);
            return ComUtils.GetInt32s(ref ppErrors, serverHandles.Length, true);
        }

        public void Dispose()
        {
            _impl.Dispose();
            if (_connectionPoint != null)
            {
                _connectionPoint.Unadvise();
                _connectionPoint.Dispose();
                _connectionPoint = null;
            }
        }

        public void GetState(out int updateRate, out bool isActive, out string name, out int timeBias, out float percentDeadband, out int localeId, out int clientHandle, out int serverHandle)
        {
            int pActive = 0;
            GroupStateMgt.GetState(out updateRate, out pActive, out name, out timeBias, out percentDeadband, out localeId, out clientHandle, out serverHandle);
            isActive = (pActive != 0);
        }

        public OpcItemState[] Read(OpcDataSource source, int[] serverHandles)
        {
            var ppItemValues = IntPtr.Zero;
            var ppErrors = IntPtr.Zero;
            SyncIO.Read((OPCDATASOURCE)source, serverHandles.Length, serverHandles, out ppItemValues, out ppErrors);

            var errors = ComUtils.GetInt32s(ref ppErrors, serverHandles.Length, true);
            var values = GetStructures<OPCITEMSTATE>(ppItemValues, serverHandles.Length)
                .Zip(errors, (v, e) => new OpcItemState
                {
                    ClientHandle = v.hClient,
                    DataValue = v.vDataValue,
                    ErrorCode = e,
                    Quality = v.wQuality,
                    Timestamp = ComUtils.GetDateTime(v.ftTimeStamp)
                })
                .ToArray();

            return values;
        }

        public void Refresh(int connection, OpcDataSource source, out int transactionId)
        {
            AsyncIO.Refresh(connection, (OPCDATASOURCE)source, out transactionId);
        }

        public int[] RemoveItems(int[] serverHandles)
        {
            var ppErrors = IntPtr.Zero;
            ItemMgt.RemoveItems(serverHandles.Length, serverHandles, out ppErrors);
            return ComUtils.GetInt32s(ref ppErrors, serverHandles.Length, true);
        }

        public int[] SetActiveState(int[] serverHandles, bool isActive)
        {
            var ppErrors = IntPtr.Zero;
            ItemMgt.SetActiveState(serverHandles.Length, serverHandles, isActive ? 1 : 0, out ppErrors);
            return ComUtils.GetInt32s(ref ppErrors, serverHandles.Length, true);
        }

        public int[] SetClientHandles(int[] serverHandles, int[] clientHandles)
        {
            var ppErrors = IntPtr.Zero;
            ItemMgt.SetClientHandles(serverHandles.Length, serverHandles, clientHandles, out ppErrors);
            return ComUtils.GetInt32s(ref ppErrors, serverHandles.Length, true);
        }

        public int[] SetDataTypes(int[] serverHandles, short[] requestedDataTypes)
        {
            var ppErrors = IntPtr.Zero;
            ItemMgt.SetDatatypes(serverHandles.Length, serverHandles, requestedDataTypes, out ppErrors);
            return ComUtils.GetInt32s(ref ppErrors, serverHandles.Length, true);
        }

        public void SetName(string name)
        {
            GroupStateMgt.SetName(name);
        }

        public void SetState(int requestedUpdateRate, out int revisedUpdateRate, bool isActive, int timeBias, float percentDeadband, int localeId, int clientHandle)
        {
            var hReqUpdateRate = GCHandle.Alloc(requestedUpdateRate, GCHandleType.Pinned);
            var hActive = GCHandle.Alloc(isActive ? 1 : 0, GCHandleType.Pinned);
            var hTimeBias = GCHandle.Alloc(timeBias, GCHandleType.Pinned);
            var hDeadband = GCHandle.Alloc(percentDeadband, GCHandleType.Pinned);
            var hLcid = GCHandle.Alloc(localeId, GCHandleType.Pinned);
            var hHandle = GCHandle.Alloc(clientHandle, GCHandleType.Pinned);

            try
            {
                GroupStateMgt.SetState(
                    hReqUpdateRate.AddrOfPinnedObject(),
                    out revisedUpdateRate,
                    hActive.AddrOfPinnedObject(),
                    hTimeBias.AddrOfPinnedObject(),
                    hDeadband.AddrOfPinnedObject(),
                    hLcid.AddrOfPinnedObject(),
                    hHandle.AddrOfPinnedObject());
            }
            finally
            {
                hReqUpdateRate.Free();
                hActive.Free();
                hTimeBias.Free();
                hDeadband.Free();
                hLcid.Free();
                hHandle.Free();
            }
        }

        public int[] Write(int[] serverHandles, object[] values)
        {
            var ppErrors = IntPtr.Zero;
            SyncIO.Write(serverHandles.Length, serverHandles, values, out ppErrors);
            return ComUtils.GetInt32s(ref ppErrors, serverHandles.Length, true);
        }

        private IEnumerable<T> GetStructures<T>(IntPtr address, int count) where T : struct
        {
            try
            {
                var p = address;
                for (int i = 0; i < count; i++, p += Marshal.SizeOf<T>())
                {
                    var r = Marshal.PtrToStructure<T>(p);
                    try
                    {
                        yield return r;
                    }
                    finally
                    {
                        Marshal.DestroyStructure<T>(p);
                    }
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(address);
            }
        }

        private void OnCancelComplete(CancelCompleteEventArgs e)
        {
            CancelComplete?.Invoke(this, e);
        }

        private void OnDataChange(DataChangeEventArgs e)
        {
            DataChange?.Invoke(this, e);
        }

        private void OnReadComplete(ReadCompleteEventArgs e)
        {
            ReadComplete?.Invoke(this, e);
        }

        private void OnWriteComplete(WriteCompleteEventArgs e)
        {
            WriteComplete?.Invoke(this, e);
        }

        private class OPCDataCallbackImpl : IOPCDataCallback
        {
            private OpcGroup _group;

            public OPCDataCallbackImpl(OpcGroup group)
            {
                _group = group;
            }

            public void OnCancelComplete(int dwTransid, int hGroup)
            {
                _group.OnCancelComplete(new CancelCompleteEventArgs(dwTransid, hGroup));
            }

            public void OnDataChange(int dwTransid, int hGroup, int hrMasterquality, int hrMastererror, int dwCount, int[] phClientItems, object[] pvValues, short[] pwQualities, System.Runtime.InteropServices.ComTypes.FILETIME[] pftTimeStamps, int[] pErrors)
            {
                var items = new OpcItemState[dwCount];
                for (int i = 0; i < dwCount; i++)
                {
                    var item = new OpcItemState();
                    item.ClientHandle = phClientItems[i];
                    item.DataValue = pvValues[i];
                    item.ErrorCode = pErrors[i];
                    item.Quality = pwQualities[i];
                    item.Timestamp = ComUtils.GetDateTime(pftTimeStamps[i]);
                    items[i] = item;
                }
                _group.OnDataChange(new DataChangeEventArgs(dwTransid, hGroup, hrMasterquality, hrMastererror, items));
            }

            public void OnReadComplete(int dwTransid, int hGroup, int hrMasterquality, int hrMastererror, int dwCount, int[] phClientItems, object[] pvValues, short[] pwQualities, System.Runtime.InteropServices.ComTypes.FILETIME[] pftTimeStamps, int[] pErrors)
            {
                var items = new OpcItemState[dwCount];
                for (int i = 0; i < dwCount; i++)
                {
                    var item = new OpcItemState();
                    item.ClientHandle = phClientItems[i];
                    item.DataValue = pvValues[i];
                    item.ErrorCode = pErrors[i];
                    item.Quality = pwQualities[i];
                    item.Timestamp = ComUtils.GetDateTime(pftTimeStamps[i]);
                    items[i] = item;
                }
                _group.OnReadComplete(new ReadCompleteEventArgs(dwTransid, hGroup, hrMasterquality, hrMastererror, items));
            }

            public void OnWriteComplete(int dwTransid, int hGroup, int hrMastererr, int dwCount, int[] pClienthandles, int[] pErrors)
            {
                _group.OnWriteComplete(new WriteCompleteEventArgs(dwTransid, hGroup, hrMastererr, dwCount, pClienthandles, pErrors));
            }
        }

        private class OpcGroupImpl : ComObject, IOPCItemMgt, IOPCSyncIO, IOPCGroupStateMgt, IOPCAsyncIO
        {
            public OpcGroupImpl(object unknown)
            {
                Unknown = unknown;
            }

            void IOPCAsyncIO.Cancel(int dwTransactionID)
            {
                var methodName = nameof(IOPCAsyncIO) + "." + nameof(IOPCAsyncIO.Cancel);
                try
                {
                    var server = BeginComCall<IOPCAsyncIO>(methodName, true);
                    server.Cancel(dwTransactionID);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            void IOPCAsyncIO.Read(int dwConnection, OPCDATASOURCE dwSource, int dwCount, int[] phServer, out int pTransactionID, out IntPtr ppErrors)
            {
                var methodName = nameof(IOPCAsyncIO) + "." + nameof(IOPCAsyncIO.Read);
                try
                {
                    var server = BeginComCall<IOPCAsyncIO>(methodName, true);
                    server.Read(dwConnection, dwSource, dwConnection, phServer, out pTransactionID, out ppErrors);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            void IOPCAsyncIO.Refresh(int dwConnection, OPCDATASOURCE dwSource, out int pTransactionID)
            {
                var methodName = nameof(IOPCAsyncIO) + "." + nameof(IOPCAsyncIO.Refresh);
                try
                {
                    var server = BeginComCall<IOPCAsyncIO>(methodName, true);
                    server.Refresh(dwConnection, dwSource, out pTransactionID);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            void IOPCAsyncIO.Write(int dwConnection, int dwCount, int[] phServer, object[] pItemValues, out int pTransactionID, out IntPtr ppErrors)
            {
                var methodName = nameof(IOPCAsyncIO) + "." + nameof(IOPCAsyncIO.Write);
                try
                {
                    var server = BeginComCall<IOPCAsyncIO>(methodName, true);
                    server.Write(dwConnection, dwCount, phServer, pItemValues, out pTransactionID, out ppErrors);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            void IOPCGroupStateMgt.CloneGroup(string szName, ref Guid riid, out object ppUnk)
            {
                var methodName = nameof(IOPCGroupStateMgt) + "." + nameof(IOPCGroupStateMgt.CloneGroup);
                try
                {
                    var server = BeginComCall<IOPCGroupStateMgt>(methodName, true);
                    server.CloneGroup(szName, ref riid, out ppUnk);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            void IOPCGroupStateMgt.GetState(out int pUpdateRate, out int pActive, out string ppName, out int pTimeBias, out float pPercentDeadband, out int pLCID, out int phClientGroup, out int phServerGroup)
            {
                var methodName = nameof(IOPCGroupStateMgt) + "." + nameof(IOPCGroupStateMgt.GetState);
                try
                {
                    var server = BeginComCall<IOPCGroupStateMgt>(methodName, true);
                    server.GetState(out pUpdateRate, out pActive, out ppName, out pTimeBias, out pPercentDeadband, out pLCID, out phClientGroup, out phServerGroup);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            void IOPCGroupStateMgt.SetName(string szName)
            {
                var methodName = nameof(IOPCGroupStateMgt) + "." + nameof(IOPCGroupStateMgt.SetName);
                try
                {
                    var server = BeginComCall<IOPCGroupStateMgt>(methodName, true);
                    server.SetName(szName);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            void IOPCGroupStateMgt.SetState(IntPtr pRequestedUpdateRate, out int pRevisedUpdateRate, IntPtr pActive, IntPtr pTimeBias, IntPtr pPercentDeadband, IntPtr pLCID, IntPtr phClientGroup)
            {
                var methodName = nameof(IOPCGroupStateMgt) + "." + nameof(IOPCGroupStateMgt.SetState);
                try
                {
                    var server = BeginComCall<IOPCGroupStateMgt>(methodName, true);
                    server.SetState(pRequestedUpdateRate, out pRevisedUpdateRate, pActive, pTimeBias, pPercentDeadband, pLCID, phClientGroup);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            void IOPCItemMgt.AddItems(int dwCount, OPCITEMDEF[] pItemArray, out IntPtr ppAddResults, out IntPtr ppErrors)
            {
                var methodName = nameof(IOPCItemMgt) + "." + nameof(IOPCItemMgt.AddItems);
                try
                {
                    var server = BeginComCall<IOPCItemMgt>(methodName, true);
                    server.AddItems(dwCount, pItemArray, out ppAddResults, out ppErrors);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            void IOPCItemMgt.CreateEnumerator(ref Guid riid, out object ppUnk)
            {
                var methodName = nameof(IOPCItemMgt) + "." + nameof(IOPCItemMgt.CreateEnumerator);
                try
                {
                    var server = BeginComCall<IOPCItemMgt>(methodName, true);
                    server.CreateEnumerator(ref riid, out ppUnk);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            void IOPCItemMgt.RemoveItems(int dwCount, int[] phServer, out IntPtr ppErrors)
            {
                var methodName = nameof(IOPCItemMgt) + "." + nameof(IOPCItemMgt.RemoveItems);
                try
                {
                    var server = BeginComCall<IOPCItemMgt>(methodName, true);
                    server.RemoveItems(dwCount, phServer, out ppErrors);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            void IOPCItemMgt.SetActiveState(int dwCount, int[] phServer, int bActive, out IntPtr ppErrors)
            {
                var methodName = nameof(IOPCItemMgt) + "." + nameof(IOPCItemMgt.SetActiveState);
                try
                {
                    var server = BeginComCall<IOPCItemMgt>(methodName, true);
                    server.SetActiveState(dwCount, phServer, bActive, out ppErrors);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            void IOPCItemMgt.SetClientHandles(int dwCount, int[] phServer, int[] phClient, out IntPtr ppErrors)
            {
                var methodName = nameof(IOPCItemMgt) + "." + nameof(IOPCItemMgt.SetClientHandles);
                try
                {
                    var server = BeginComCall<IOPCItemMgt>(methodName, true);
                    server.SetClientHandles(dwCount, phServer, phClient, out ppErrors);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            void IOPCItemMgt.SetDatatypes(int dwCount, int[] phServer, short[] pRequestedDatatypes, out IntPtr ppErrors)
            {
                var methodName = nameof(IOPCItemMgt) + "." + nameof(IOPCItemMgt.SetDatatypes);
                try
                {
                    var server = BeginComCall<IOPCItemMgt>(methodName, true);
                    server.SetDatatypes(dwCount, phServer, pRequestedDatatypes, out ppErrors);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            void IOPCItemMgt.ValidateItems(int dwCount, OPCITEMDEF[] pItemArray, int bBlobUpdate, out IntPtr ppValidationResults, out IntPtr ppErrors)
            {
                var methodName = nameof(IOPCItemMgt) + "." + nameof(IOPCItemMgt.ValidateItems);
                try
                {
                    var server = BeginComCall<IOPCItemMgt>(methodName, true);
                    server.ValidateItems(dwCount, pItemArray, bBlobUpdate, out ppValidationResults, out ppErrors);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            void IOPCSyncIO.Read(OPCDATASOURCE dwSource, int dwCount, int[] phServer, out IntPtr ppItemValues, out IntPtr ppErrors)
            {
                var methodName = nameof(IOPCSyncIO) + "." + nameof(IOPCSyncIO.Read);
                try
                {
                    var server = BeginComCall<IOPCSyncIO>(methodName, true);
                    server.Read(dwSource, dwCount, phServer, out ppItemValues, out ppErrors);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }

            void IOPCSyncIO.Write(int dwCount, int[] phServer, object[] pItemValues, out IntPtr ppErrors)
            {
                var methodName = nameof(IOPCSyncIO) + "." + nameof(IOPCSyncIO.Write);
                try
                {
                    var server = BeginComCall<IOPCSyncIO>(methodName, true);
                    server.Write(dwCount, phServer, pItemValues, out ppErrors);
                }
                finally
                {
                    EndComCall(methodName);
                }
            }
        }
    }
}