using System.Collections.ObjectModel;
using MegaApi;
using MegaDesktop.ViewModels;

namespace MegaDesktop.Services
{
    internal class TransferManager
    {
        private readonly IDispatcher _dispatcher;
        private readonly ObservableCollection<TransferHandleViewModel> _transfers;

        public TransferManager(ObservableCollection<TransferHandleViewModel> transfers, IDispatcher dispatcher)
        {
            _transfers = transfers;
            _dispatcher = dispatcher;
        }

        public ObservableCollection<TransferHandleViewModel> Transfers
        {
            get { return _transfers; }
        }

        public void CancelAllTransfers()
        {
            foreach (TransferHandleViewModel transfer in Transfers)
                transfer.CancelTransfer();
        }

        public void Remove(TransferHandleViewModel transfer)
        {
            _dispatcher.InvokeOnUiThread(() =>
                Transfers.Remove(transfer));
        }

        public void AddNewTransfer(TransferHandle transfer)
        {
            _dispatcher.InvokeOnUiThread(() =>
                Transfers.Add(new TransferHandleViewModel(transfer, this, _dispatcher)));
        }
    }
}