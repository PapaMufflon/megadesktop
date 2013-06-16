using System.Collections.Generic;
using System.Threading.Tasks;

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
    }
}