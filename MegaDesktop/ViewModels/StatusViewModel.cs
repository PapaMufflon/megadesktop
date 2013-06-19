using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MegaApi;
using MegaDesktop.Services;

namespace MegaDesktop.ViewModels
{
    internal class StatusViewModel : INotifyPropertyChanged
    {
        private readonly IDispatcher _dispatcher;
        private Status _currentStatus;
        private string _message;

        public StatusViewModel(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public string Message
        {
            get { return _message; }
            private set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public Status CurrentStatus
        {
            get { return _currentStatus; }
            private set
            {
                _currentStatus = value;
                OnCurrentStatusChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<EventArgs> CurrentStatusChanged;

        public void SetStatus(Status newStatus)
        {
            switch (newStatus)
            {
                case Status.Communicating:
                    Message = "Communicating with mega...";
                    break;
                case Status.Loaded:
                    Message = "Done.";
                    EmptyInAMoment();
                    break;
                case Status.LoggingIn:
                    Message = "Logging in...";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("newStatus");
            }

            CurrentStatus = newStatus;
        }

        private void EmptyInAMoment()
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(2000);
                _dispatcher.InvokeOnUiThread(() => Message = string.Empty);
            });
        }

        public void Error(MegaApiException exception)
        {
            Message = String.Format("Error: {0}({1})", exception.ErrorNumber, exception.Message);
            CurrentStatus = Status.Loaded;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnCurrentStatusChanged()
        {
            EventHandler<EventArgs> handler = CurrentStatusChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}