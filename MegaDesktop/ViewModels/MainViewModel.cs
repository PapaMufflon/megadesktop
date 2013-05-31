using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
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

            Status = new StatusViewModel();
            Status.SetStatus(Services.Status.Communicating);

            RootNode = new NodeViewModel(dispatcher);
            var apiManager = new MegaApiWrapper();
            var refreshService = new RefreshService(Status, apiManager, this);

            Transfers = new ObservableCollection<TransferHandleViewModel>();
            var transferManager = new TransferManager(Transfers, dispatcher, this);

            var userAccount = new UserAccount(apiManager, Status, refreshService, title, transferManager);
            userAccount.LoginLastUser().ContinueWith(x =>
            {
                if (x.Exception == null)
                    return;

                MessageBox.Show("Error while loading account: " + x.Exception);
                Application.Current.Shutdown();
            });

            UploadCommand = new UploadCommand(apiManager, Status, this, dispatcher, transferManager, refreshService);
            UploadFolderCommand = new UploadFolderCommand(apiManager, Status, this, this, dispatcher);
            DownloadCommand = new DownloadCommand(apiManager, Status, this, dispatcher, transferManager, refreshService);
            DeleteCommand = new DeleteCommand(apiManager, Status, refreshService, this, dispatcher);
            LoginCommand = new LoginCommand(apiManager, userAccount);
            LogoutCommand = new LogoutCommand(transferManager, this, userAccount);
            RefreshCommand = new RefreshCommand(refreshService);
            SelectedListNodeActionCommand = new SelectedListNodeActionCommand(DownloadCommand as DownloadCommand, refreshService);
        }

        public ICanSetStatus Status { get; private set; }
        public ICommand UploadCommand { get; private set; }
        public ICommand UploadFolderCommand { get; private set; }
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

        protected virtual void OnSelectedNodeChanged()
        {
            var handler = SelectedNodeChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}