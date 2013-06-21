using MegaDesktop.Services;

namespace MegaDesktop.ViewModels
{
    internal class MainViewModel
    {
        public MainViewModel(NodeManager nodeManager,
                             TransfersViewModel transfersViewModel,
                             ToolBarViewModel toolBarViewModel,
                             HeaderViewModel headerViewModel)
        {
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