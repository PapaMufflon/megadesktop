using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public Task<IEnumerable<MegaNode>> GetNodes()
        {
            return _api.GetNodesAsync();
        }

        // wtf: http://stackoverflow.com/a/11969255/453024
        public Task<bool> RemoveNode(string targetNodeId)
        {
            return _api.RemoveNodeAsync(targetNodeId);
        }

        public Task<TransferHandle> UploadFile(string targetNodeId, string filename)
        {
            return _api.UploadFileAsync(targetNodeId, filename);
        }

        public Task<MegaNode> CreateFolder(string targetNode, string folderName)
        {
            return _api.CreateFolderAsync(targetNode, folderName);
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