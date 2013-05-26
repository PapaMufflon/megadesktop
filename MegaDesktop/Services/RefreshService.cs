using System.Linq;
using MegaDesktop.ViewModels;

namespace MegaDesktop.Services
{
    internal class RefreshService : ICanRefresh
    {
        private readonly ICanSetStatus _status;
        private readonly IMegaApi _megaApi;
        private readonly IHaveNodes _nodes;

        public RefreshService(ICanSetStatus status, IMegaApi megaApi, IHaveNodes nodes)
        {
            _status = status.AssertIsNotNull("status");
            _megaApi = megaApi.AssertIsNotNull("megaApi");
            _nodes = nodes.AssertIsNotNull("nodes");
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

            _megaApi.GetNodes(nodes =>
            {
                _nodes.RootNode.Update(nodes);
                _nodes.SelectedListNode = node == null
                                                ? _nodes.RootNode.Children.Single(n => n.Type == NodeType.RootFolder)
                                                : _nodes.RootNode.Descendant(node.Id);
                
                _status.SetStatus(Status.Loaded);
            }, e => _status.Error(e));
        }
    }
}