using System;
using System.Windows.Input;
using MegaDesktop.ViewModels;

namespace MegaDesktop.Commands
{
    public class RemoveCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly IManageTransfers _transfers;
        
        public RemoveCommand(IManageTransfers transfers)
        {
            _transfers = transfers;
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

        public virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}