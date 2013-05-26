using System;
using System.Collections.Generic;
using MegaApi;
using MegaApi.Comms.Requests;

namespace MegaDesktop.Services
{
    internal interface IMegaApi
    {
        MegaUser User { get; }

        void Use(Mega mega);

        IMegaRequest GetNodes(Action<List<MegaNode>> onSuccess, Action<int> onError);
        IMegaRequest RemoveNode(string targetNodeId, Action onSuccess, Action<int> onError);

        IMegaRequest UploadFile(string targetNodeId, string filename, Action<UploadHandle> onHandleReady, Action<int> onError);
        IMegaRequest DownloadFile(MegaNode node, string filename, Action<DownloadHandle> onHandleReady, Action<int> onError);
        
        void SaveAccount(string filePath);
    }
}