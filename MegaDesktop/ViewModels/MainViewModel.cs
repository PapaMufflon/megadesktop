using System;
using System.Windows.Input;
using MegaDesktop.Commands;
using MegaDesktop.Services;
using MegaWpf;

namespace MegaDesktop.ViewModels
{
    internal class MainViewModel : ISelectedNodeListener
    {
        public event EventHandler<EventArgs> SelectedNodeChanged;

        private NodeViewModel _selectedTreeNode;
        private NodeViewModel _selectedListNode;

        public MainViewModel(ITodo todo, IDispatcher dispatcher)
        {
            Status = new StatusViewModel { Message = "Retrieving the list of files..." };
            UploadCommand = new UploadCommand(todo.Api, Status, this, todo, dispatcher);
            DownloadCommand = new DownloadCommand(todo.Api, Status, todo, this, dispatcher);
            RootNode = new NodeViewModel(null, dispatcher);
        }

        public ICanSetStatus Status { get; set; }
        public ICommand UploadCommand { get; set; }
        public ICommand DownloadCommand { get; set; }
        public NodeViewModel RootNode { get; private set; }
        public NodeViewModel SelectedNode { get; set; }

        public NodeViewModel SelectedTreeNode
        {
            get { return _selectedTreeNode; }
            set
            {
                _selectedTreeNode = value;
                SelectedNode = SelectedTreeNode;
                OnSelectedNodeChanged();
            }
        }

        public NodeViewModel SelectedListNode
        {
            get { return _selectedListNode; }
            set
            {
                _selectedListNode = value;
                SelectedNode = SelectedListNode;
                OnSelectedNodeChanged();
            }
        }

        protected virtual void OnSelectedNodeChanged()
        {
            var handler = SelectedNodeChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}