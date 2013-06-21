using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MegaApi;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;
using Microsoft.Win32;

namespace MegaDesktop.Commands
{
    internal class UploadCommand : ICommand, IToolBarCommand
    {
        private readonly IDispatcher _dispatcher;
        private readonly RefreshService _refresh;
        private readonly MegaApiWrapper _megaApiWrapper;
        private readonly StatusViewModel _status;
        private readonly TransferManager _transfers;

        public UploadCommand(StatusViewModel status,
                             IDispatcher dispatcher,
                             TransferManager transfers,
                             RefreshService refresh,
                             MegaApiWrapper megaApiWrapper)
        {
            _status = status;
            _dispatcher = dispatcher;
            _transfers = transfers;
            _refresh = refresh;
            _megaApiWrapper = megaApiWrapper;

            _status.CurrentStatusChanged += (s, e) => _dispatcher.InvokeOnUiThread(OnCanExecuteChanged);
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

            UploadFile(d.FileName, currentNode);
        }

        public void UploadFile(string fileName, NodeViewModel node)
        {
            _status.SetStatus(Status.Communicating);

            _megaApiWrapper.UploadFile(node.Id, fileName)
                           .ContinueWith(TakeCareOfTransferHandle);
        }

        private void TakeCareOfTransferHandle(Task<TransferHandle> task)
        {
            if (task.Exception == null)
            {
                var transfer = task.Result;
                transfer.TransferEnded += (s, e) => _refresh.RefreshCurrentNode();
                _transfers.AddNewTransfer(transfer);

                _status.SetStatus(Status.Loaded);
            }
            else
                _status.Error(task.Exception.InnerExceptions.First() as MegaApiException);
        }

        public bool CanExecute(object parameter)
        {
            var node = parameter as NodeViewModel;

            return node != null &&
                   node.Type != NodeType.File &&
                   _status.CurrentStatus == Status.Loaded;
        }

        public virtual void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public int Position { get { return 0; } }
        public bool Gap { get { return true; } }
        public string ImageSource { get { return "pack://application:,,,/MegaDesktop;component/resources/UploadFile.png"; } }
        public string ToolTip { get { return Resource.UploadFile; } }
    }
}