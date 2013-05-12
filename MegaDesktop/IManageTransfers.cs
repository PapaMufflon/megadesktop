using System.Collections.ObjectModel;
using MegaApi;

namespace MegaWpf
{
    public interface IManageTransfers
    {
        ObservableCollection<TransferHandle> Transfers { get; }
        void CancelAllTransfers();
    }
}