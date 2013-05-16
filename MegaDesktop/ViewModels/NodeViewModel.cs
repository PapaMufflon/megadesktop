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
        private readonly IDispatcher _dispatcher;

        public NodeViewModel(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher.AssertIsNotNull("dispatcher");

            Children = new ObservableCollection<NodeViewModel>();
            ChildNodes = new ObservableCollection<NodeViewModel>();
        }

        public NodeViewModel(IDispatcher dispatcher, MegaNode node) : this(dispatcher)
        {
            _node = node.AssertIsNotNull("node");
        }

        public ObservableCollection<NodeViewModel> Children { get; private set; }
        public ObservableCollection<NodeViewModel> ChildNodes { get; private set; }
        public string Id { get { return _node != null ? _node.Id : string.Empty; } }
        public string Name { get { return _node != null ? _node.Attributes.Name : string.Empty; } }
        public MegaNode HideMe { get { return _node; } }

        public void Update(List<MegaNode> nodes)
        {
            SetChildrenRecursive(this, nodes);
        }

        private void SetChildrenRecursive(NodeViewModel nodeViewModel, List<MegaNode> nodes)
        {
            //hier können die nodes durcheinander kommen --> klappt dann net mit dem Aufbau...
            foreach (var node in nodes)
            {
                if (node.ParentId == nodeViewModel.Id)
                {
                    var childViewModel = SetAsChild(nodeViewModel, node);
                    SetChildrenRecursive(childViewModel, nodes);
                }
            }

            foreach (var node in nodeViewModel.Children.Reverse().Where(x => nodes.All(y => x.Id != y.Id)))
                _dispatcher.InvokeOnUiThread(() =>
                    nodeViewModel.Children.Remove(node));
        }

        private NodeViewModel SetAsChild(NodeViewModel nodeViewModel, MegaNode child)
        {
            var childViewModel = nodeViewModel.Children.SingleOrDefault(x => child.Id == x.Id);

            if (childViewModel != null)
                childViewModel.SetNode(child);
            else
            {
                childViewModel = new NodeViewModel(_dispatcher, child);

                _dispatcher.InvokeOnUiThread(() =>
                {
                    nodeViewModel.Children.Add(childViewModel);

                    if (child.Type != MegaNodeType.File)
                        nodeViewModel.ChildNodes.Add(childViewModel);
                });
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