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
        private readonly MegaApiWrapper _megaApiWrapper;
        private readonly RefreshService _refresh;
        private readonly StatusViewModel _status;
        private readonly TransferManager _transfers;

        public DownloadCommand(MegaApiWrapper megaApiWrapper, StatusViewModel status, NodeManager nodes,
                               IDispatcher dispatcher, TransferManager transfers, RefreshService refresh)
        {
            _megaApiWrapper = megaApiWrapper;
            _status = status;
            _transfers = transfers;
            _refresh = refresh;

            nodes.SelectedNodeChanged += (s, e) => dispatcher.InvokeOnUiThread(OnCanExecuteChanged);
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            var node = parameter as NodeViewModel;

            return node != null &&
                   node.Type == NodeType.File;
        }

        public void Execute(object parameter)
        {
            MegaNode node = (parameter as NodeViewModel).MegaNode;

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
                _megaApiWrapper.DownloadFile(clickedNode, d.FileName, OnHandleReady, e => _status.Error(e));
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
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}