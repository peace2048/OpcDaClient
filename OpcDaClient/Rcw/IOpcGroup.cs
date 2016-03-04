using System;

namespace OpcDaClient.Rcw
{
    public interface IOpcGroup : IDisposable
    {
        OpcItemResult[] AddItems(OpcItemDefine[] items);
        void GetState(out int updateRate, out bool isActive, out string name, out int timeBias, out float percentDeadband, out int localeId, out int clientHandle, out int serverHandle);
        OpcItemState[] Read(OpcDataSource source, int[] serverHandles);
        int[] RemoveItems(int[] serverHandles);
        int[] SetActiveState(int[] serverHandles, bool isActive);
        int[] SetClientHandles(int[] serverHandles, int[] clientHandles);
        int[] SetDataTypes(int[] serverHandles, short[] requestedDataTypes);
        void SetName(string name);
        void SetState(int requestedUpdateRate, out int revisedUpdateRate, bool isActive, int timeBias, float percentDeadband, int localeId, int clientHandle);
        int[] Write(int[] serverHandles, object[] values);
    }
}