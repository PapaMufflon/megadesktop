using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MegaApi;
using MegaDesktop.Commands;
using MegaDesktop.Services;

namespace MegaDesktop.ViewModels
{
    internal class TransferHandleViewModel : INotifyPropertyChanged
    {
        private readonly TransferHandle _transferHandle;

        public TransferHandleViewModel(TransferHandle transferHandle, TransferManager transfers, IDispatcher dispatcher)
        {
            _transferHandle = transferHandle;
            _transferHandle.PropertyChanged += (s, e) =>
                {
                    ((RemoveCommand) RemoveCommand).RaiseCanExecuteChanged();

                    OnPropertyChanged(e.PropertyName);
                };

            RemoveCommand = new RemoveCommand(transfers, dispatcher);
            CancelCommand = new CancelCommand();
        }

        public ICommand RemoveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public string Name
        {
            get { return _transferHandle.Node.Attributes.Name; }
        }

        public double Progress
        {
            get { return _transferHandle.Progress; }
        }

        public TransferHandleStatus Status
        {
            get { return _transferHandle.Status; }
        }

        public double? Speed
        {
            get { return _transferHandle.Speed; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void CancelTransfer()
        {
            _transferHandle.CancelTransfer();
        }
    }
}