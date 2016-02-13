using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;

namespace OpcDaClient.RcwWrapper
{
    internal class OpcGroup : Opc.Ua.Com.Client.ComObject, IOpcGroup, OpcRcw.Da.IOPCDataCallback
    {
        private IOpcDataCallback _callback;

        public OpcGroup(object unknown)
        {
            this.Unknown = unknown;
        }

        public void AddItems(OpcItemDefine[] items)
        {
            var count = items.Length;
            var defs = items.Select(_ => new OpcRcw.Da.OPCITEMDEF
            {
                bActive = _.IsActive ? 1 : 0,
                hClient = _.ClientHandle,
                szItemID = _.ItemId,
                szAccessPath = string.Empty,
                vtRequestedDataType = (short)VarEnum.VT_EMPTY
            })
            .ToArray();
            var ppResults = IntPtr.Zero;
            var ppErrors = IntPtr.Zero;
            const string methodName = "";
            try
            {
                var server = BeginComCall<OpcRcw.Da.IOPCItemMgt>(methodName, true);
                server.AddItems(count, defs, out ppResults, out ppErrors);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                EndComCall(methodName);
            }
            var errors = Opc.Ua.Com.ComUtils.GetInt32s(ref ppErrors, count, true);
            if (ppResults != IntPtr.Zero)
            {
                var pointer = ppResults;
                for (int i = 0; i < count; i++)
                {
                    var item = items[i];
                    item.ErrorCode = errors[i];

                    var result = Marshal.PtrToStructure<OpcRcw.Da.OPCITEMRESULT>(pointer);
                    item.ServerHandle = result.hServer;
                    item.DataType = (VarEnum)result.vtCanonicalDataType;

                    if (result.pBlob != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(result.pBlob);
                        result.pBlob = IntPtr.Zero;
                        result.dwBlobSize = 0;
                    }

                    Marshal.DestroyStructure<OpcRcw.Da.OPCITEMRESULT>(pointer);
                    pointer += Marshal.SizeOf<OpcRcw.Da.OPCITEMRESULT>();
                }
                Marshal.FreeCoTaskMem(ppResults);
            }
        }

        public void OnCancelComplete(int dwTransid, int hGroup)
        {
            throw new NotImplementedException();
        }

        public void OnDataChange(int dwTransid, int hGroup, int hrMasterquality, int hrMastererror, int dwCount, int[] phClientItems, object[] pvValues, short[] pwQualities, System.Runtime.InteropServices.ComTypes.FILETIME[] pftTimeStamps, int[] pErrors)
        {
            if (_callback != null && dwCount > 0)
            {
                _callback.OnDataChanged(dwTransid,
                    Enumerable.Range(0, dwCount)
                        .Select(index => new OpcDataChanged
                        {
                            ClientHandle = phClientItems[index],
                            Value = Opc.Ua.Com.ComUtils.ProcessComValue(pvValues[index]),
                            Quality = pwQualities[index],
                            Timestamp = Opc.Ua.Com.ComUtils.GetDateTime(pftTimeStamps[index]),
                            ErrorCode = pErrors[index]
                        })
                        .ToArray());
            }
        }

        public void OnReadComplete(int dwTransid, int hGroup, int hrMasterquality, int hrMastererror, int dwCount, int[] phClientItems, object[] pvValues, short[] pwQualities, System.Runtime.InteropServices.ComTypes.FILETIME[] pftTimeStamps, int[] pErrors)
        {
            throw new NotImplementedException();
        }

        public void OnWriteComplete(int dwTransid, int hGroup, int hrMastererr, int dwCount, int[] pClienthandles, int[] pErrors)
        {
            throw new NotImplementedException();
        }

        public DaValue[] Read(int[] serverHandles)
        {
            var count = serverHandles.Length;
            if (count < 1)
            {
                return null;
            }
            var ppValues = IntPtr.Zero;
            var ppErrors = IntPtr.Zero;
            const string methodName = "IOPCSyncIO.Read";
            try
            {
                var server = BeginComCall<OpcRcw.Da.IOPCSyncIO>(methodName, true);
                server.Read(OpcRcw.Da.OPCDATASOURCE.OPC_DS_DEVICE, count, serverHandles, out ppValues, out ppErrors);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                EndComCall(methodName);
            }
            var results = new DaValue[count];

            var errors = Opc.Ua.Com.ComUtils.GetInt32s(ref ppErrors, count, true);
            if (ppValues != IntPtr.Zero)
            {
                IntPtr pointer = ppValues;
                for (int i = 0; i < count; i++)
                {
                    var result = new DaValue();
                    results[i] = result;

                    result.ErrorCode = errors[i];

                    var rawValue = Marshal.PtrToStructure<OpcRcw.Da.OPCITEMSTATE>(pointer);
                    result.Value = Opc.Ua.Com.ComUtils.ProcessComValue(rawValue.vDataValue);
                    result.Quality = rawValue.wQuality;
                    result.Timestamp = Opc.Ua.Com.ComUtils.GetDateTime(rawValue.ftTimeStamp);

                    Marshal.DestroyStructure<OpcRcw.Da.OPCITEMSTATE>(pointer);

                    pointer += Marshal.SizeOf<OpcRcw.Da.OPCITEMSTATE>();
                }
                Marshal.FreeCoTaskMem(ppValues);
            }
            return results;
        }

        public IDisposable Watch(IOpcDataCallback callback)
        {
            _callback = callback;
            var conn = new Opc.Ua.Com.Client.ConnectionPoint(Unknown, typeof(OpcRcw.Da.IOPCDataCallback).GUID);
            conn.Advise(this);
            return Disposable.Create(() =>
            {
                conn.Unadvise();
                conn.Dispose();
                _callback = null;
            });
        }

        public int[] Write(int[] serverHandles, object[] values)
        {
            var errors = IntPtr.Zero;
            const string methodName = "IOPCSyncIO.Write";
            try
            {
                var server = BeginComCall<OpcRcw.Da.IOPCSyncIO>(methodName, true);
                server.Write(serverHandles.Length, serverHandles, values, out errors);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                EndComCall(methodName);
            }
            return Opc.Ua.Com.ComUtils.GetInt32s(ref errors, serverHandles.Length, true);
        }
    }
}