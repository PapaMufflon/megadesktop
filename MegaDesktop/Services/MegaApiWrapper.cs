using System;
using System.Collections.Generic;
using MegaApi;

namespace MegaDesktop.Services
{
    internal class MegaApiWrapper
    {
        private Mega _api;

        public MegaUser User
        {
            get { return _api != null ? _api.User : null; }
        }

        public void Register(Mega api)
        {
            _api = api;
        }

        public void GetNodes(Action<List<MegaNode>> onSuccess, Action<int> onError)
        {
            _api.GetNodes(onSuccess, onError);
        }

        public void RemoveNode(string targetNodeId, Action onSuccess, Action<int> onError)
        {
            _api.RemoveNode(targetNodeId, onSuccess, onError);
        }

        public void UploadFile(string targetNodeId, string filename, Action<UploadHandle> onHandleReady, Action<int> onError)
        {
            _api.UploadFile(targetNodeId, filename, onHandleReady, onError);
        }

        public void CreateFolder(string targetNodeId, string folderName, Action<MegaNode> onSuccess, Action<int> onError)
        {
            _api.CreateFolder(targetNodeId, folderName, onSuccess, onError);
        }

        public void DownloadFile(MegaNode node, string filename, Action<DownloadHandle> onHandleReady, Action<int> onError)
        {
            _api.DownloadFile(node, filename, onHandleReady, onError);
        }

        public void SaveAccount(string filePath)
        {
            _api.SaveAccount(filePath);
        }
    }
}