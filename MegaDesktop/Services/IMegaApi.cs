using System;
using System.Collections.Generic;
using MegaApi;
using MegaApi.Comms.Requests;

namespace MegaDesktop.Services
{
    internal interface IMegaApi
    {
        MegaUser User { get; }

        void Init(MegaUser user, Action onSuccess, Action<int> onError);
        MegaUser LoadAccount(string filePath);

        IMegaRequest GetNodes(Action<List<MegaNode>> onSuccess, Action<int> onError);
        IMegaRequest RemoveNode(string targetNodeId, Action onSuccess, Action<int> onError);

        IMegaRequest UploadFile(string targetNodeId, string filename, Action<UploadHandle> onHandleReady, Action<int> onError);
        IMegaRequest DownloadFile(MegaNode node, string filename, Action<DownloadHandle> onHandleReady, Action<int> onError);
        IMegaRequest CreateFolder(string targetNodeId, string folderName, Action<MegaNode> OnSuccess, Action<int> OnError);
        
        void SaveAccount(string filePath);
    }
}