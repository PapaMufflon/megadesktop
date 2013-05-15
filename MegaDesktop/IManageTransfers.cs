using System.Collections.ObjectModel;
using MegaApi;
using MegaDesktop.ViewModels;

namespace MegaDesktop
{
    internal interface IManageTransfers
    {
        ObservableCollection<TransferHandleViewModel> Transfers { get; }
        void CancelAllTransfers();
        void Remove(TransferHandleViewModel transfer);
        void AddNewTransfer(TransferHandle transfer);
    }
}