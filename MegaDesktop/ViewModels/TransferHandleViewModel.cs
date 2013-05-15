using System.ComponentModel;
using System.Runtime.CompilerServices;
using MegaApi;
using MegaDesktop.Commands;

namespace MegaDesktop.ViewModels
{
    public class TransferHandleViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly TransferHandle _transferHandle;
        
        public TransferHandleViewModel(TransferHandle transferHandle, IManageTransfers transfers)
        {
            _transferHandle = transferHandle;
            _transferHandle.PropertyChanged += (s, e) =>
            {
                RemoveCommand.OnCanExecuteChanged();

                OnPropertyChanged(e.PropertyName);
            };

            RemoveCommand = new RemoveCommand(transfers);
        }

        public RemoveCommand RemoveCommand { get; private set; }

        public string Name { get { return _transferHandle.Node.Attributes.Name; } }
        public double Progress { get { return _transferHandle.Progress; } }
        public TransferHandleStatus Status { get { return _transferHandle.Status; } }
        public double? Speed { get { return _transferHandle.Speed; } }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void CancelTransfer()
        {
            _transferHandle.CancelTransfer();
        }
    }
}