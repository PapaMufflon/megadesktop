using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MegaDesktop.Services;

namespace MegaDesktop.ViewModels
{
    internal class StatusViewModel : INotifyPropertyChanged
    {
        private Status _currentStatus;
        private string _message;

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
                    break;
                case Status.LoggingIn:
                    Message = "Logging in...";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("newStatus");
            }

            CurrentStatus = newStatus;
        }

        public void Error(int errorNumber)
        {
            Message = String.Format("Error: {0}", errorNumber);
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