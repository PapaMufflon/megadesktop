using System;
using System.Windows.Input;
using MegaApi;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;
using Microsoft.Win32;
using Ninject;

namespace MegaDesktop.Commands
{
    internal class UploadCommand : ICommand
    {
        private readonly IDispatcher _dispatcher;
        private readonly RefreshService _refresh;
        private readonly MegaApiWrapper _megaApiWrapper;
        private readonly StatusViewModel _status;
        private readonly TransferManager _transfers;

        public UploadCommand(StatusViewModel status, NodeManager nodes,
                             IDispatcher dispatcher, TransferManager transfers, RefreshService refresh,
                             MegaApiWrapper megaApiWrapper)
        {
            _status = status;
            _dispatcher = dispatcher;
            _transfers = transfers;
            _refresh = refresh;
            _megaApiWrapper = megaApiWrapper;

            _status.CurrentStatusChanged += (s, e) => _dispatcher.InvokeOnUiThread(OnCanExecuteChanged);
            nodes.SelectedNodeChanged += (s, e) => dispatcher.InvokeOnUiThread(OnCanExecuteChanged);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var currentNode = parameter as NodeViewModel;

            if (currentNode == null)
                return;

            var d = new OpenFileDialog();

            if (d.ShowDialog() != true)
                return;

            _status.SetStatus(Status.Communicating);
            _megaApiWrapper.UploadFile(currentNode.Id, d.FileName, OnHandleReady, err => _status.Error(err));
        }

        public bool CanExecute(object parameter)
        {
            var node = parameter as NodeViewModel;

            return node != null &&
                   node.Type != NodeType.File &&
                   _status.CurrentStatus == Status.Loaded;
        }

        private void OnHandleReady(TransferHandle transfer)
        {
            transfer.TransferEnded += (s, e) => _refresh.RefreshCurrentNode();
            _transfers.AddNewTransfer(transfer);

            _status.SetStatus(Status.Loaded);
        }

        protected virtual void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}