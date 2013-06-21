using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaApi;
using MegaDesktop.Util;
using MegaDesktop.ViewModels;

namespace MegaDesktop.Services
{
    internal class RefreshService
    {
        private readonly MegaApiWrapper _megaApiWrapper;
        private readonly NodeManager _nodes;
        private readonly StatusViewModel _status;

        public RefreshService(StatusViewModel status, NodeManager nodes, MegaApiWrapper megaApiWrapper)
        {
            _status = status.AssertIsNotNull("status");
            _nodes = nodes.AssertIsNotNull("nodes");
            _megaApiWrapper = megaApiWrapper.AssertIsNotNull("megaApiWrapper");
        }

        public void RefreshCurrentNode()
        {
            Refresh(_nodes.SelectedListNode);
        }

        public void Reload()
        {
            Refresh();
        }

        private void Refresh(NodeViewModel node = null)
        {
            _status.SetStatus(Status.Communicating);

            _megaApiWrapper.GetNodes()
                           .ContinueWith(x => UpdateNodes(x, node));
        }

        private void UpdateNodes(Task<IEnumerable<MegaNode>> task, NodeViewModel currentNode)
        {
            if (task.Exception != null)
            {
                _status.Error(task.Exception.InnerExceptions.First() as MegaApiException);
                return;
            }

            _nodes.RootNode.Update(task.Result.ToList());
            _nodes.SelectedListNode = currentNode == null
                                          ? _nodes.RootNode.Children.Single(n => n.Type == NodeType.RootFolder)
                                          : _nodes.RootNode.Descendant(currentNode.Id);

            _status.SetStatus(Status.Loaded);
        }
    }
}