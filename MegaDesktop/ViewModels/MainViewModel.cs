using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MegaApi;
using MegaDesktop.Commands;
using MegaDesktop.Services;

namespace MegaDesktop.ViewModels
{
    internal class MainViewModel : ISelectedNodeListener, IManageTransfers, IHaveTheRootNode
    {
        public event EventHandler<EventArgs> SelectedNodeChanged;

        private NodeViewModel _selectedTreeNode;
        private NodeViewModel _selectedListNode;

        public MainViewModel(ITodo todo, ICanRefresh refresh, IDispatcher dispatcher, ICanSetTitle title)
        {
            Status = new StatusViewModel { Message = "Retrieving the list of files..." };

            var apiManager = new ApiManager();
            var userAccount = new UserAccount(apiManager, Status, refresh, title);
            userAccount.AutoLoginLastUser();

            UploadCommand = new UploadCommand(todo.Api, Status, this, todo, dispatcher);
            DownloadCommand = new DownloadCommand(todo.Api, Status, todo, this, dispatcher);
            DeleteCommand = new DeleteCommand(todo.Api, Status, refresh);
            LoginCommand = new LoginCommand(apiManager, userAccount, this, title, refresh);
            LogoutCommand = new LogoutCommand(this, this, userAccount);
            SelectedListNodeActionCommand = new SelectedListNodeActionCommand(DownloadCommand as DownloadCommand, refresh);
            RootNode = new NodeViewModel(null, dispatcher);
            Transfers = new ObservableCollection<TransferHandle>();
        }

        public ICanSetStatus Status { get; private set; }
        public ICommand UploadCommand { get; private set; }
        public ICommand DownloadCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand LoginCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }
        public ICommand SelectedListNodeActionCommand { get; private set; }
        public NodeViewModel RootNode { get; private set; }
        public NodeViewModel SelectedNode { get; set; }
        public ObservableCollection<TransferHandle> Transfers { get; private set; }
        
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

        public void CancelAllTransfers()
        {
            foreach (var transfer in Transfers)
            {
                transfer.CancelTransfer();
            }
        }
    }
}