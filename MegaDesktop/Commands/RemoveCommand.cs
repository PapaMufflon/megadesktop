using System;
using System.Windows.Input;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;

namespace MegaDesktop.Commands
{
    internal class RemoveCommand : ICommand
    {
        private readonly IDispatcher _dispatcher;
        private readonly TransferManager _transfers;

        public RemoveCommand(TransferManager transfers, IDispatcher dispatcher)
        {
            _transfers = transfers;
            _dispatcher = dispatcher;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return (parameter as TransferHandleViewModel) != null;
        }

        public void Execute(object parameter)
        {
            var transfer = parameter as TransferHandleViewModel;

            _transfers.Remove(transfer);
        }

        public void RaiseCanExecuteChanged()
        {
            _dispatcher.InvokeOnUiThread(OnCanExecuteChanged);
        }

        protected virtual void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}