using System;

namespace OpcDaClient.Rcw
{
    public interface IOpcServer : IDisposable
    {
        bool IsSupportAsyncGroup { get; }

        IOpcGroupAsync AddAsyncGroup(string name, bool isActive, int requestedUpdateRate, int clientHandle, float deadband, int localeId);

        IOpcGroup AddGroup(string name, bool isActive, int requestedUpdateRate, int clientHandle, float deadband, int localeId);

        string[] BrowseAccessPaths(string itemId);

        string[] BrowseItemIds(OpcBrowseType browseType, string filterCriteria, short dataTypeFilter, int accessRightsFilter);

        void ChangeBrowsePosition(OpcBrowseDirection direction, string name);

        string GetErrorString(int errorCode);

        string GetErrorString(int errorCode, int localeId);

        IOpcGroup GetGroupByName(string name);

        string GetItemId(string itemDataId);

        int GetLocaleId();

        OpcServerStatus GetStatus();

        int[] QueryAvailableLocaleIDs();

        void QueryAvailableProperties(string itemId);

        void RemoveGroup(int serverHandle, bool force);

        void SetClientName(string name);

        void SetLocalId(int localeId);
    }
}