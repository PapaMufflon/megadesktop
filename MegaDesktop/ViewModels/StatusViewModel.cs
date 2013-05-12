using System;
using System.ComponentModel;
using MegaDesktop.Commands;

namespace MegaDesktop.ViewModels
{
    internal class StatusViewModel : INotifyPropertyChanged, ICanSetStatus
    {
        private string _message;
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

        public void Set(string newStatus)
        {
            Message = newStatus;
        }

        public void Done()
        {
            Message = "Done.";
        }

        public void Error(int errorNumber)
        {
            Message = String.Format("Error: {0}", errorNumber);
        }
    }
}