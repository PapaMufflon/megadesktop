using System;
using MegaDesktop.ViewModels;

namespace MegaDesktop.Services
{
    internal class NodeManager
    {
        public event EventHandler<EventArgs> SelectedNodeChanged;

        private NodeViewModel _selectedListNode;
        private NodeViewModel _selectedNode;
        private NodeViewModel _selectedTreeNode;

        public NodeManager(IDispatcher dispatcher)
        {
            RootNode = new NodeViewModel(dispatcher);
        }

        public NodeViewModel RootNode { get; private set; }

        public NodeViewModel SelectedNode
        {
            get { return _selectedNode; }
            set
            {
                _selectedNode = value;
                OnSelectedNodeChanged();
            }
        }

        public NodeViewModel SelectedTreeNode
        {
            get { return _selectedTreeNode; }
            set
            {
                _selectedTreeNode = value;
                SelectedNode = SelectedTreeNode;
            }
        }

        public NodeViewModel SelectedListNode
        {
            get { return _selectedListNode; }
            set
            {
                _selectedListNode = value;
                SelectedNode = SelectedListNode;
            }
        }

        protected virtual void OnSelectedNodeChanged()
        {
            var handler = SelectedNodeChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
