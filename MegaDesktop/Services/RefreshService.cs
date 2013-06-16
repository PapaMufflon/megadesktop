using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaApi;
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
                           .ContinueWith(x =>
                               {
                                   if (x.Exception != null)
                                   {
                                       _status.Error(x.Exception.InnerExceptions.First() as MegaApiException);
                                       return;
                                   }

                                   _nodes.RootNode.Update(x.Result.ToList());
                                   _nodes.SelectedListNode = node == null
                                                                 ? _nodes.RootNode.Children.Single(n => n.Type == NodeType.RootFolder)
                                                                 : _nodes.RootNode.Descendant(node.Id);

                                   _status.SetStatus(Status.Loaded);
                               });
        }
    }
}