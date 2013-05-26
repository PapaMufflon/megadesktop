using System.Collections.ObjectModel;
using MegaApi;
using MegaDesktop.ViewModels;

namespace MegaDesktop.Services
{
    internal class TransferManager : IManageTransfers
    {
        private readonly ObservableCollection<TransferHandleViewModel> _transfers;

        public TransferManager(ObservableCollection<TransferHandleViewModel> transfers)
        {
            _transfers = transfers;
        }

        public ObservableCollection<TransferHandleViewModel> Transfers { get { return _transfers; } }

        public void CancelAllTransfers()
        {
            foreach (var transfer in Transfers)
            {
                transfer.CancelTransfer();
            }
        }

        public void Remove(TransferHandleViewModel transfer)
        {
            _dispatcher.InvokeOnUiThread(() =>
                                         Transfers.Remove(transfer));
        }

        public void AddNewTransfer(TransferHandle transfer)
        {
            _dispatcher.InvokeOnUiThread(() =>
                                         Transfers.Add(new TransferHandleViewModel(transfer, this, _dispatcher, this)));
        }
    }
}