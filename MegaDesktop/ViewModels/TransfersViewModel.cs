using System.Collections.ObjectModel;
using MegaDesktop.Services;

namespace MegaDesktop.ViewModels
{
    internal class TransfersViewModel
    {
        private readonly TransferManager _transferManager;

        public TransfersViewModel(TransferManager transferManager)
        {
            _transferManager = transferManager;
        }

        public ObservableCollection<TransferHandleViewModel> Transfers { get { return _transferManager.Transfers; } }
    }
}