using System;
using System.Windows.Input;
using MegaApi;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;
using Microsoft.Win32;

namespace MegaDesktop.Commands
{
    internal class UploadCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly IHaveTheApi _api;
        private readonly ICanSetStatus _status;
        private readonly IDispatcher _dispatcher;
        private readonly IManageTransfers _transfers;
        private readonly ICanRefresh _refresh;

        public UploadCommand(IHaveTheApi api, ICanSetStatus status, ISelectedNodeListener selectedNodeListener, IDispatcher dispatcher, IManageTransfers transfers, ICanRefresh refresh)
        {
            _api = api;
            _status = status;
            _dispatcher = dispatcher;
            _transfers = transfers;
            _refresh = refresh;

            _status.CurrentStatusChanged += (s, e) => _dispatcher.InvokeOnUiThread(OnCanExecuteChanged);
            selectedNodeListener.SelectedNodeChanged += (s, e) => dispatcher.InvokeOnUiThread(OnCanExecuteChanged);
        }

        public void Execute(object parameter)
        {
            var currentNode = parameter as NodeViewModel;

            if (currentNode == null)
                return;

            var d = new OpenFileDialog();

            if (d.ShowDialog() != true)
                return;

            _status.SetStatus(Status.Communicating);
            _api.Api.UploadFile(currentNode.Id, d.FileName, OnHandleReady, err => _status.Error(err));
        }

        private void OnHandleReady(TransferHandle transfer)
        {
            transfer.TransferEnded += (s, e) => _refresh.RefreshCurrentNode();
            _transfers.AddNewTransfer(transfer);

            _status.SetStatus(Status.Loaded);
        }

        public bool CanExecute(object parameter)
        {
            var node = parameter as NodeViewModel;

            return node != null &&
                   node.Type != NodeType.File &&
                   _status.CurrentStatus == Status.Loaded;
        }

        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}