using System;
using System.Windows.Input;
using MegaApi;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;
using Microsoft.Win32;

namespace MegaDesktop.Commands
{
    internal class DownloadCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly IHaveTheApi _api;
        private readonly ICanSetStatus _status;
        private readonly IManageTransfers _transfers;
        private readonly ICanRefresh _refresh;

        public DownloadCommand(IHaveTheApi api, ICanSetStatus status, ISelectedNodeListener selectedNodeListener, IDispatcher dispatcher, IManageTransfers transfers, ICanRefresh refresh)
        {
            _api = api;
            _status = status;
            _transfers = transfers;
            _refresh = refresh;

            selectedNodeListener.SelectedNodeChanged += (s, e) => dispatcher.InvokeOnUiThread(OnCanExecuteChanged);
        }

        public bool CanExecute(object parameter)
        {
            var node = parameter as NodeViewModel;

            return node != null &&
                   node.HideMe.Type == MegaNodeType.File;
        }

        public void Execute(object parameter)
        {
            var node = (parameter as NodeViewModel).HideMe;

            if (node.Type == MegaNodeType.File)
            {
                DownloadFile(node);
            }
            else
            {
                // todo
            }

            // todo if multiselect
        }

        private void DownloadFile(MegaNode clickedNode)
        {
            var d = new SaveFileDialog();
            d.FileName = clickedNode.Attributes.Name;
            if (d.ShowDialog() == true)
            {
                _status.SetStatus(Status.Communicating);
                _api.Api.DownloadFile(clickedNode, d.FileName, OnHandleReady, e => _status.Error(e));
            }
        }

        private void OnHandleReady(TransferHandle transfer)
        {
            transfer.TransferEnded += (s, e) => _refresh.RefreshCurrentNode();
            _transfers.AddNewTransfer(transfer);

            _status.SetStatus(Status.Loaded);
        }
        
        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}