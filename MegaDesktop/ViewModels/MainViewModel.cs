using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MegaDesktop.Commands;
using MegaDesktop.Services;

namespace MegaDesktop.ViewModels
{
    internal class MainViewModel : ISelectedNodeListener, IHaveNodes, INotifyPropertyChanged
    {
        public event EventHandler<EventArgs> SelectedNodeChanged;

        private NodeViewModel _selectedNode;
        private NodeViewModel _selectedTreeNode;
        private NodeViewModel _selectedListNode;

        public MainViewModel(IDispatcher dispatcher, ICanSetTitle title)
        {
            dispatcher.AssertIsNotNull("dispatcher");
            title.AssertIsNotNull("title");

            Status = new StatusViewModel { Message = "Retrieving the list of files..." };
            RootNode = new NodeViewModel(dispatcher);
            
            var apiManager = new ApiManager();
            var refreshService = new RefreshService(Status, apiManager, this);

            var userAccount = new UserAccount(apiManager, Status, refreshService, title);
            userAccount.AutoLoginLastUser();

            Transfers = new ObservableCollection<TransferHandleViewModel>();
            var transferManager = new TransferManager(Transfers, dispatcher, this);

            UploadCommand = new UploadCommand(apiManager, Status, this, dispatcher, transferManager, refreshService);
            DownloadCommand = new DownloadCommand(apiManager, Status, this, dispatcher, transferManager, refreshService);
            DeleteCommand = new DeleteCommand(apiManager, Status, refreshService, this, dispatcher);
            LoginCommand = new LoginCommand(apiManager, userAccount, transferManager, title, refreshService);
            LogoutCommand = new LogoutCommand(transferManager, this, userAccount);
            RefreshCommand = new RefreshCommand(refreshService);
            SelectedListNodeActionCommand = new SelectedListNodeActionCommand(DownloadCommand as DownloadCommand, refreshService);
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
        public ObservableCollection<TransferHandleViewModel> Transfers { get; private set; }

        public NodeViewModel SelectedNode
        {
            get { return _selectedNode; }
            set
            {
                _selectedNode = value;
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}