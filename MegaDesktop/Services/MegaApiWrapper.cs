using System;
using System.Collections.Generic;
using MegaApi;
using MegaApi.Comms.Requests;

namespace MegaDesktop.Services
{
    internal class MegaApiWrapper : IMegaApi
    {
        private Mega _mega;

        public MegaUser User { get { return _mega.AssertIsNotNull().User; } }

        public void Use(Mega mega)
        {
            _mega = mega;
        }

        public IMegaRequest GetNodes(Action<List<MegaNode>> onSuccess, Action<int> onError)
        {
            return _mega.AssertIsNotNull().GetNodes(onSuccess, onError);
        }

        public IMegaRequest RemoveNode(string targetNodeId, Action onSuccess, Action<int> onError)
        {
            return _mega.AssertIsNotNull().RemoveNode(targetNodeId, onSuccess, onError);
        }

        public IMegaRequest UploadFile(string targetNodeId, string filename, Action<UploadHandle> onHandleReady, Action<int> onError)
        {
            return _mega.AssertIsNotNull().UploadFile(targetNodeId, filename, onHandleReady, onError);
        }

        public IMegaRequest DownloadFile(MegaNode node, string filename, Action<DownloadHandle> onHandleReady, Action<int> onError)
        {
            return _mega.AssertIsNotNull().DownloadFile(node, filename, onHandleReady, onError);
        }

        public void SaveAccount(string filePath)
        {
            _mega.AssertIsNotNull().SaveAccount(filePath);
        }
    }

    internal static class MegaShouldNotBeNull
    {
        public static Mega AssertIsNotNull(this Mega self)
        {
            if (self == null)
                throw new InvalidOperationException("The MEGA-API has not been set yet. You cannot do this action.");

            return self;
        }
    }
}