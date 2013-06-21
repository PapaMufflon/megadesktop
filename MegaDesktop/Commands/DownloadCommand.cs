using System;
using System.Linq;
using System.Windows.Input;
using MegaApi;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;
using Microsoft.Win32;

namespace MegaDesktop.Commands
{
    internal class DownloadCommand : ICommand, IToolBarCommand
    {
        private readonly MegaApiWrapper _megaApiWrapper;
        private readonly StatusViewModel _status;
        private readonly TransferManager _transfers;

        public DownloadCommand(MegaApiWrapper megaApiWrapper, StatusViewModel status, TransferManager transfers)
        {
            _megaApiWrapper = megaApiWrapper;
            _status = status;
            _transfers = transfers;
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
            var node = (parameter as NodeViewModel).MegaNode;

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
            var d = new SaveFileDialog { FileName = clickedNode.Attributes.Name };

            if (d.ShowDialog() != true)
                return;

            _status.SetStatus(Status.Communicating);
            _megaApiWrapper.DownloadFile(clickedNode, d.FileName)
                           .ContinueWith(x =>
                               {
                                   if (x.Exception != null)
                                       _status.Error(x.Exception.InnerExceptions.First() as MegaApiException);
                                   else
                                       OnHandleReady(x.Result);
                               });
        }

        private void OnHandleReady(TransferHandle transfer)
        {
            _transfers.AddNewTransfer(transfer);

            _status.SetStatus(Status.Loaded);
        }

        public void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;

            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public int Position { get { return 2; } }
        public bool Gap { get { return true; } }
        public string ImageSource { get { return "pack://application:,,,/MegaDesktop;component/resources/Download.png"; } }
        public string ToolTip { get { return Resource.Download; } }
    }
}