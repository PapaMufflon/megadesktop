using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MegaApi;
using MegaDesktop.Commands;
using MegaDesktop.Services;

namespace MegaDesktop.ViewModels
{
    internal class MainViewModel : ISelectedNodeListener, IManageTransfers, IHaveTheRootNode, ICanRefresh
    {
        public event EventHandler<EventArgs> SelectedNodeChanged;

        private NodeViewModel _selectedTreeNode;
        private NodeViewModel _selectedListNode;
        private readonly ApiManager _apiManager;

        public MainViewModel(IDispatcher dispatcher, ICanSetTitle title)
        {
            Status = new StatusViewModel { Message = "Retrieving the list of files..." };

            _apiManager = new ApiManager();
            var userAccount = new UserAccount(_apiManager, Status, this, title);
            userAccount.AutoLoginLastUser();

            UploadCommand = new UploadCommand(_apiManager, Status, this, dispatcher, this, this);
            DownloadCommand = new DownloadCommand(_apiManager, Status, this, dispatcher, this, this);
            DeleteCommand = new DeleteCommand(_apiManager, Status, this);
            LoginCommand = new LoginCommand(_apiManager, userAccount, this, title, this);
            LogoutCommand = new LogoutCommand(this, this, userAccount);
            RefreshCommand = new RefreshCommand(this);
            SelectedListNodeActionCommand = new SelectedListNodeActionCommand(DownloadCommand as DownloadCommand, this);
            RootNode = new NodeViewModel(null, dispatcher);
            Transfers = new ObservableCollection<TransferHandleViewModel>();
        }

        public ICanSetStatus Status { get; private set; }
        public ICommand UploadCommand { get; private set; }
        public ICommand DownloadCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand LoginCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand SelectedListNodeActionCommand { get; private set; }
        public NodeViewModel RootNode { get; private set; }
        public NodeViewModel SelectedNode { get; set; }
        public ObservableCollection<TransferHandleViewModel> Transfers { get; private set; }
        
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

        public void Remove(TransferHandleViewModel transfer)
        {
            Transfers.Remove(transfer);
        }

        public void AddNewTransfer(TransferHandle transfer)
        {
            Transfers.Add(new TransferHandleViewModel(transfer, this));
        }

        public void RefreshCurrentNode()
        {
            Refresh(SelectedListNode);
        }

        public void Reload()
        {
            Refresh();
        }

        private void Refresh(NodeViewModel node = null)
        {
            Status.SetStatus(Services.Status.Communicating);

            _apiManager.Api.GetNodes(nodes =>
            {
                Status.SetStatus(Services.Status.Loaded);
                RootNode.Update(nodes);

                SelectedListNode = node == null
                                       ? RootNode.Children.Single(n => n.HideMe.Type == MegaNodeType.RootFolder)
                                       : RootNode.Descendant(node.Id);
            }, e => Status.Error(e));
        }
    }
}