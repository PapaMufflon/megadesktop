using MegaDesktop.Services;
using MegaDesktop.Util;

namespace MegaDesktop.ViewModels
{
    internal class MainViewModel
    {
        private readonly NodeManager _nodeManager;

        public MainViewModel(NodeManager nodeManager, TransfersViewModel transfersViewModel, ToolBarViewModel toolBarViewModel, HeaderViewModel headerViewModel)
        {
            _nodeManager = nodeManager.AssertIsNotNull("nodeManager");
            TransfersViewModel = transfersViewModel;
            ToolBarViewModel = toolBarViewModel;
            HeaderViewModel = headerViewModel;
            NodeManager = nodeManager;
        }

        public NodeManager NodeManager { get; private set; }
        public TransfersViewModel TransfersViewModel { get; private set; }
        public ToolBarViewModel ToolBarViewModel { get; private set; }
        public HeaderViewModel HeaderViewModel { get; private set; }
    }
}