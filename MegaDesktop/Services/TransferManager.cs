using System.Collections.ObjectModel;
using MegaApi;
using MegaDesktop.ViewModels;

namespace MegaDesktop.Services
{
    internal class TransferManager : IManageTransfers
    {
        private readonly ObservableCollection<TransferHandleViewModel> _transfers;
        private readonly IDispatcher _dispatcher;
        private readonly ISelectedNodeListener _listener;

        public TransferManager(ObservableCollection<TransferHandleViewModel> transfers, IDispatcher dispatcher, ISelectedNodeListener listener)
        {
            _transfers = transfers;
            _dispatcher = dispatcher;
            _listener = listener;
        }

        public ObservableCollection<TransferHandleViewModel> Transfers { get { return _transfers; } }

        public void CancelAllTransfers()
        {
            foreach (var transfer in Transfers)
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
                Transfers.Add(new TransferHandleViewModel(transfer, this, _dispatcher, _listener)));
        }
    }
}