using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MegaDesktop.ViewModels;

namespace MegaDesktop.Services
{
    internal class NodeManager : INotifyPropertyChanged
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
                OnPropertyChanged();
            }
        }

        public NodeViewModel SelectedTreeNode
        {
            get { return _selectedTreeNode; }
            set
            {
                _selectedTreeNode = value;
                SelectedNode = SelectedTreeNode;
                OnPropertyChanged();
            }
        }

        public NodeViewModel SelectedListNode
        {
            get { return _selectedListNode; }
            set
            {
                _selectedListNode = value;
                SelectedNode = SelectedListNode;
                OnPropertyChanged();
            }
        }

        protected virtual void OnSelectedNodeChanged()
        {
            var handler = SelectedNodeChanged;

            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
