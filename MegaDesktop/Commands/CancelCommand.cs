using System;
using System.Windows;
using System.Windows.Input;
using MegaApi;
using MegaDesktop.ViewModels;

namespace MegaDesktop.Commands
{
    internal class CancelCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        
        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            var transfer = parameter as TransferHandleViewModel;

            if ((transfer.Status == TransferHandleStatus.Downloading || transfer.Status == TransferHandleStatus.Uploading))
            {
                var type = (transfer.Status == TransferHandleStatus.Downloading ? "download" : "upload");
                var text = String.Format("Are you sure to cancel the {0} process for {1}?", type, transfer.Name);

                if (MessageBox.Show(text, "Cancel " + type, MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;
            }

            transfer.CancelTransfer();
        }

        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
