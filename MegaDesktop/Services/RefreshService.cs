using System.Linq;
using MegaDesktop.ViewModels;

namespace MegaDesktop.Services
{
    internal class RefreshService : ICanRefresh
    {
        private readonly ICanSetStatus _status;
        private readonly IHaveTheApi _apiManager;
        private readonly IHaveNodes _nodes;

        public RefreshService(ICanSetStatus status, IHaveTheApi apiManager, IHaveNodes nodes)
        {
            _status = status;
            _apiManager = apiManager;
            _nodes = nodes;
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

            _apiManager.Api.GetNodes(nodes =>
                {
                    _status.SetStatus(Status.Loaded);
                    _nodes.RootNode.Update(nodes);

                    _nodes.SelectedListNode = node == null
                                                  ? _nodes.RootNode.Children.Single(n => n.Type == NodeType.RootFolder)
                                                  : _nodes.RootNode.Descendant(node.Id);
                }, e => _status.Error(e));
        }
    }
}