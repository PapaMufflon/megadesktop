using System;
using System.Windows.Input;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;

namespace MegaDesktop.Commands
{
    internal class RemoveCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly IManageTransfers _transfers;
        private readonly IDispatcher _dispatcher;

        public RemoveCommand(IManageTransfers transfers, IDispatcher dispatcher)
        {
            _transfers = transfers;
            _dispatcher = dispatcher;
        }

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
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}