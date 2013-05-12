using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MegaApi;
using MegaDesktop.Services;

namespace MegaDesktop.ViewModels
{
    internal class NodeViewModel
    {
        private MegaNode _node;
        private readonly NodeViewModel _parent;
        private readonly IDispatcher _dispatcher;

        public NodeViewModel(NodeViewModel parent, IDispatcher dispatcher)
        {
            _parent = parent;
            _dispatcher = dispatcher;
            Children = new ObservableCollection<NodeViewModel>();
        }

        public NodeViewModel(NodeViewModel parent, IDispatcher dispatcher, MegaNode node) : this(parent, dispatcher)
        {
            _node = node;
        }

        public ObservableCollection<NodeViewModel> Children { get; private set; }
        public NodeViewModel Parent { get { return _parent; } }
        public string Id { get { return _node != null ? _node.Id : string.Empty; } }
        public string Name { get { return _node != null ? _node.Attributes.Name : string.Empty; } }
        public MegaNode HideMe { get { return _node; } }

        public void Update(List<MegaNode> nodes)
        {
            SetChildrenRecursive(this, nodes);
        }

        private void SetChildrenRecursive(NodeViewModel nodeViewModel, List<MegaNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node.ParentId == nodeViewModel.Id)
                {
                    var childViewModel = SetAsChild(nodeViewModel, node);
                    SetChildrenRecursive(childViewModel, nodes);
                }
            }
        }

        private NodeViewModel SetAsChild(NodeViewModel nodeViewModel, MegaNode child)
        {
            var childViewModel = nodeViewModel.Children.SingleOrDefault(x => child.Id == x.Id);

            if (childViewModel != null)
                childViewModel.SetNode(child);
            else
            {
                childViewModel = new NodeViewModel(nodeViewModel, _dispatcher, child);

                _dispatcher.InvokeOnUiThread(() => nodeViewModel.Children.Add(childViewModel));
            }

            return childViewModel;
        }

        private void SetNode(MegaNode child)
        {
            _node = child;
        }

        public NodeViewModel Descendant(string id)
        {
            var descendant = Children.FirstOrDefault(x => x.Id == id);

            return descendant ?? Children.Select(child => child.Descendant(id)).FirstOrDefault(x => x != null);
        }
    }
}