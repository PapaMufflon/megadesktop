using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MegaDesktop.Commands;
using MegaDesktop.Services;
using Ninject;

namespace MegaDesktop.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private readonly NodeManager _nodeManager;

        public MainViewModel(StatusViewModel status, NodeManager nodeManager)
        {
            Status = status.AssertIsNotNull("status");
            _nodeManager = nodeManager.AssertIsNotNull("nodeManager");

            Status.SetStatus(Services.Status.Communicating);
            Transfers = new ObservableCollection<TransferHandleViewModel>();
        }

        public StatusViewModel Status { get; private set; }
        public ObservableCollection<TransferHandleViewModel> Transfers { get; private set; }

        [Inject]
        public UploadCommand UploadCommand { get; set; }

        [Inject]
        public UploadFolderCommand UploadFolderCommand { get; set; }

        [Inject]
        public DownloadCommand DownloadCommand { get; set; }

        [Inject]
        public DeleteCommand DeleteCommand { get; set; }

        [Inject]
        public LoginCommand LoginCommand { get; set; }

        [Inject]
        public LogoutCommand LogoutCommand { get; set; }

        [Inject]
        public RefreshCommand RefreshCommand { get; set; }

        [Inject]
        public SelectedListNodeActionCommand SelectedListNodeActionCommand { get; set; }

        public NodeViewModel SelectedNode
        {
            get { return _nodeManager.SelectedNode; }
            set
            {
                _nodeManager.SelectedNode = value;
                OnPropertyChanged();
            }
        }

        public NodeViewModel SelectedTreeNode
        {
            get { return _nodeManager.SelectedTreeNode; }
            set
            {
                _nodeManager.SelectedTreeNode = value;
                OnPropertyChanged();
            }
        }

        public NodeViewModel SelectedListNode
        {
            get { return _nodeManager.SelectedListNode; }
            set
            {
                _nodeManager.SelectedListNode = value;
                OnPropertyChanged();
            }
        }

        public NodeViewModel RootNode
        {
            get { return _nodeManager.RootNode; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}