using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MegaApi;
using MegaDesktop.Services;

namespace MegaDesktop.ViewModels
{
    internal class NodeViewModel
    {
        private readonly IDispatcher _dispatcher;
        private MegaNode _node;

        public NodeViewModel(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher.AssertIsNotNull("dispatcher");

            Children = new ObservableCollection<NodeViewModel>();
            ChildNodes = new ObservableCollection<NodeViewModel>();
        }

        public NodeViewModel(IDispatcher dispatcher, MegaNode node)
            : this(dispatcher)
        {
            _node = node.AssertIsNotNull("node");
        }

        public ObservableCollection<NodeViewModel> Children { get; private set; }
        public ObservableCollection<NodeViewModel> ChildNodes { get; private set; }

        public string Id
        {
            get { return _node != null ? _node.Id : string.Empty; }
        }

        public string Name
        {
            get
            {
                if (_node == null)
                    return string.Empty;

                switch (Type)
                {
                    case NodeType.File:
                    case NodeType.Folder:
                    case NodeType.Dummy:
                        return _node.Attributes.Name;
                    case NodeType.RootFolder:
                        return Resource.RootNode;
                    case NodeType.Inbox:
                        return Resource.Inbox;
                    case NodeType.Trash:
                        return Resource.Trash;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public NodeType Type
        {
            get { return (NodeType) _node.Type; }
        }

        public string LastModified
        {
            get { return _node.Timestamp.HasValue ? string.Format("{0:g}", _node.Timestamp.Value) : string.Empty; }
        }

        public MegaNode MegaNode
        {
            get { return _node; }
        }

        public string Size
        {
            get { return _node.Size.HasValue ? _node.Size.Value.BytesToString() : string.Empty; }
        }

        public void Update(List<MegaNode> nodes)
        {
            SetChildrenRecursive(this, nodes);
        }

        private void SetChildrenRecursive(NodeViewModel nodeViewModel, List<MegaNode> nodes)
        {
            foreach (MegaNode node in nodes)
            {
                if (node.ParentId == nodeViewModel.Id)
                {
                    NodeViewModel childViewModel = SetAsChild(nodeViewModel, node);
                    SetChildrenRecursive(childViewModel, nodes);
                }
            }

            foreach (NodeViewModel node in nodeViewModel.Children.Reverse().Where(x => nodes.All(y => x.Id != y.Id)))
                _dispatcher.InvokeOnUiThread(() =>
                                             nodeViewModel.Children.Remove(node));
        }

        private NodeViewModel SetAsChild(NodeViewModel nodeViewModel, MegaNode child)
        {
            NodeViewModel childViewModel = nodeViewModel.Children.SingleOrDefault(x => child.Id == x.Id);

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
            NodeViewModel descendant = Children.FirstOrDefault(x => x.Id == id);

            return descendant ?? Children.Select(child => child.Descendant(id)).FirstOrDefault(x => x != null);
        }
    }
}