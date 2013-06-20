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
        private int _progress;
        private double? _speed;
        private TransferHandleStatus _status;

        public TransferHandleViewModel(TransferHandle transferHandle, IDispatcher dispatcher)
        {
            _transferHandle = transferHandle;
            _transferHandle.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case "Progress":
                        Progress = (int)_transferHandle.Progress;
                        break;

                    case "Status":
                        Status = _transferHandle.Status;
                        break;

                    case "Speed":
                        Speed = _transferHandle.Speed;
                        break;
                }
            };

            CancelCommand = new CancelCommand(this, dispatcher);
        }

        public ICommand CancelCommand { get; private set; }

        public string Name
        {
            get { return _transferHandle.Node.Attributes.Name; }
        }

        public string Size
        {
            get { return _transferHandle.Size.BytesToString(); }
        }

        public string TransferType
        {
            get
            {
                return _transferHandle is UploadHandle
                           ? Resource.Upload
                           : Resource.Download;
            }
        }

        public int Progress
        {
            get { return _progress; }
            set
            {
                if (value == Progress)
                    return;

                _progress = value;
                OnPropertyChanged();
            }
        }

        public TransferHandleStatus Status
        {
            get { return _status; }
            set
            {
                if (value == Status)
                    return;

                _status = value;
                OnPropertyChanged();
            }
        }

        public double? Speed
        {
            get { return _speed; }
            set
            {
                if (value == Speed)
                    return;

                _speed = value;
                OnPropertyChanged();
            }
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