using System;
using System.ComponentModel;
using MegaDesktop.Commands;
using MegaDesktop.Services;

namespace MegaDesktop.ViewModels
{
    internal class StatusViewModel : INotifyPropertyChanged, ICanSetStatus
    {
        private string _message;
        private Status _currentStatus;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler<EventArgs> CurrentStatusChanged;

        protected virtual void OnCurrentStatusChanged()
        {
            var handler = CurrentStatusChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

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

        public Status CurrentStatus
        {
            get { return _currentStatus; }
            private set
            {
                _currentStatus = value;
                OnCurrentStatusChanged();
            }
        }

        public void Error(int errorNumber)
        {
            Message = String.Format("Error: {0}", errorNumber);
            CurrentStatus = Status.Loaded;
        }
    }
}