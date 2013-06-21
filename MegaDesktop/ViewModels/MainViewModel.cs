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

        public MainViewModel(NodeManager nodeManager, TransfersViewModel transfersViewModel, ToolBarViewModel toolBarViewModel, HeaderViewModel headerViewModel)
        {
            _nodeManager = nodeManager.AssertIsNotNull("nodeManager");
            TransfersViewModel = transfersViewModel;
            ToolBarViewModel = toolBarViewModel;
            HeaderViewModel = headerViewModel;
            NodeManager = nodeManager;

            _nodeManager.SelectedNodeChanged += (s, e) => OnPropertyChanged("SelectedNode");
        }

        public NodeManager NodeManager { get; private set; }
        public TransfersViewModel TransfersViewModel { get; private set; }
        public ToolBarViewModel ToolBarViewModel { get; private set; }
        public HeaderViewModel HeaderViewModel { get; private set; }

        [Inject] public SelectedListNodeActionCommand SelectedListNodeActionCommand { get; set; }

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}