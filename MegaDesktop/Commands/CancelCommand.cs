using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MegaApi;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;

namespace MegaDesktop.Commands
{
    internal class CancelCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private TransferHandleViewModel _observedTransfer;
        private readonly IDispatcher _dispatcher;

        public CancelCommand(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public bool CanExecute(object parameter)
        {
            var transferHandleViewModel = parameter as TransferHandleViewModel;

            ObserveTransfer(transferHandleViewModel);

            return transferHandleViewModel != null &&
                   transferHandleViewModel.Status != TransferHandleStatus.Success &&
                   transferHandleViewModel.Status != TransferHandleStatus.Cancelled &&
                   transferHandleViewModel.Status != TransferHandleStatus.Error;
        }

        private void ObserveTransfer(TransferHandleViewModel transferHandleViewModel)
        {
            if (_observedTransfer != null)
                _observedTransfer.PropertyChanged -= OnTransferChanged;

            _observedTransfer = transferHandleViewModel;

            if (_observedTransfer != null)
                _observedTransfer.PropertyChanged += OnTransferChanged;
        }

        private void OnTransferChanged(object sender, PropertyChangedEventArgs e)
        {
            _dispatcher.InvokeOnUiThread(OnCanExecuteChanged);
        }

        public void Execute(object parameter)
        {
            var transfer = parameter as TransferHandleViewModel;

            if ((transfer.Status == TransferHandleStatus.Downloading ||
                 transfer.Status == TransferHandleStatus.Uploading))
            {
                string type = (transfer.Status == TransferHandleStatus.Downloading ? "download" : "upload");
                string text = String.Format("Are you sure to cancel the {0} process for {1}?", type, transfer.Name);

                if (MessageBox.Show(text, "Cancel " + type, MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;
            }

            transfer.CancelTransfer();
        }

        protected virtual void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}