using System.Collections.ObjectModel;
using MegaApi;

namespace MegaDesktop
{
    public interface IManageTransfers
    {
        ObservableCollection<TransferHandle> Transfers { get; }
        void CancelAllTransfers();
    }
}