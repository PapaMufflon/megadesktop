using System.Collections.Generic;
using System.Threading.Tasks;
using MegaApi.Comms.Requests;

namespace MegaApi
{
    public partial class Mega
    {
        public static Task<Mega> InitAsync(MegaUser user)
        {
            var tcs = new TaskCompletionSource<Mega>();
            Init(user,
                 tcs.SetResult,
                 errno => tcs.SetException(new MegaApiException(errno, "Login error")));

            return tcs.Task;
        }

        public Task<MegaNode> CreateFolderAsync(MegaNode target, List<MegaNode> nodes, string folder, char separator)
        {
            var tcs = new TaskCompletionSource<MegaNode>();
            CreateFolder(target, nodes, folder, separator,
                         m => tcs.SetResult(m),
                         errno =>
                         tcs.SetException(new MegaApiException(errno, string.Format("Could not create the folder {0}", folder)))
                );

            return tcs.Task;
        }

        public Task<MegaNode> CreateFolderAsync(string targetNode, string folderName)
        {
            var tcs = new TaskCompletionSource<MegaNode>();
            CreateFolder(targetNode, folderName,
                         m => tcs.SetResult(m),
                         errno =>
                         tcs.SetException(new MegaApiException(errno, string.Format("Could not create the folder {0}", folderName)))
                );

            return tcs.Task;
        }


        public Task<TransferHandle> UploadFileAsync(string targetNodeId, string filename)
        {
            var tcs = new TaskCompletionSource<TransferHandle>();
            UploadFile(targetNodeId, filename,
                       tcs.SetResult,
                       errno => tcs.SetException(new MegaApiException(MegaApiError.ESYSTEM, "Could not upload the file"))
                );

            return tcs.Task;

        }

        public Task<IEnumerable<MegaNode>> GetNodesAsync()
        {
            var tcs = new TaskCompletionSource<IEnumerable<MegaNode>>();
            GetNodes(tcs.SetResult,
                     errno => tcs.SetException(new MegaApiException((int)errno, "Could not get the list of nodes")));

            return tcs.Task;
        }

        // wtf: http://stackoverflow.com/a/11969255/453024
        public Task<bool> RemoveNodeAsync(string targetNodeId)
        {
            var tcs = new TaskCompletionSource<bool>();
            
            RemoveNode(targetNodeId,
                       () => tcs.SetResult(true),
                       errno => tcs.SetException(new MegaApiException(errno, "Could not remove the given node")));

            return tcs.Task;
        }

        public Task<DownloadHandle> DownloadFileAsync(MegaNode node, string filename)
        {
            var tcs = new TaskCompletionSource<DownloadHandle>();

            DownloadFile(node, filename,
                         tcs.SetResult,
                         errno => tcs.SetException(new MegaApiException(errno, "Could not download the given node")));

            return tcs.Task;
        }
    }
}