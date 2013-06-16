using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using MegaApi;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;

namespace MegaDesktop.Commands
{
    internal class UploadFolderCommand : ICommand
    {
        private readonly TransferManager _transfers;
        private readonly RefreshService _refresh;
        private readonly MegaApiWrapper _megaApiWrapper;
        private readonly IDispatcher _dispatcher;
        private readonly NodeManager _nodes;
        private readonly StatusViewModel _status;

        public UploadFolderCommand(StatusViewModel status, NodeManager nodes,
                                   IDispatcher dispatcher, MegaApiWrapper megaApiWrapper,
                                   TransferManager transfers, RefreshService refresh)
        {
            _status = status.AssertIsNotNull("status");
            _nodes = nodes.AssertIsNotNull("nodes");
            _dispatcher = dispatcher.AssertIsNotNull("dispatcher");
            _megaApiWrapper = megaApiWrapper.AssertIsNotNull("megaApiWrapper");
            _transfers = transfers.AssertIsNotNull("transfers");
            _refresh = refresh.AssertIsNotNull("refresh");

            nodes.SelectedNodeChanged +=
                (s, e) => dispatcher.InvokeOnUiThread(OnCanExecuteChanged);
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            var node = parameter as NodeViewModel;

            return node != null &&
                   node.Type != NodeType.File &&
                   _status.CurrentStatus == Status.Loaded;
        }

        public void Execute(object parameter)
        {
            var currentNode = parameter as NodeViewModel;

            var d = new FolderBrowserDialog();
            var result = d.ShowDialog();

            if (result != DialogResult.OK)
                return;

            CreateFolder(currentNode, d.SelectedPath);

            _refresh.RefreshCurrentNode();
        }

        private void CreateFolder(NodeViewModel node, string path)
        {
            _status.SetStatus(Status.Communicating);

            var folderName = Path.GetFileName(path);
            var child = node.Children.SingleOrDefault(x => x.Name == folderName);

            if (child != null)
                UploadFolderContent(path, child);
            else
                _megaApiWrapper.CreateFolder(node.Id, folderName)
                               .ContinueWith(x =>
                                   {
                                       if (x.Exception != null)
                                           _status.Error(x.Exception.InnerExceptions.First() as MegaApiException);
                                       else
                                           OnFolderCreated(x.Result, path);
                                   });
        }

        private void OnFolderCreated(MegaNode node, string path)
        {

            var nodeViewModel = new NodeViewModel(_dispatcher, node);

            var parent = _nodes.RootNode.Descendant(node.ParentId);

            _dispatcher.InvokeOnUiThread(() =>
                {
                    parent.Children.Add(nodeViewModel);
                    parent.ChildNodes.Add(nodeViewModel);
                });

            UploadFolderContent(path, nodeViewModel);
        }

        private void UploadFolderContent(string path, NodeViewModel nodeViewModel)
        {
            foreach (var file in Directory.EnumerateFiles(path))
            {
                var fileName = Path.GetFileName(file);

                if (nodeViewModel.Children.All(x => x.Name != fileName))
                    _megaApiWrapper.UploadFile(nodeViewModel.Id, file)
                                   .ContinueWith(x =>
                                       {
                                           if (x.Exception != null)
                                               _status.Error(x.Exception.InnerExceptions.First() as MegaApiException);
                                           else
                                               _transfers.AddNewTransfer(x.Result);
                                       });
            }

            foreach (var directory in Directory.EnumerateDirectories(path))
                CreateFolder(nodeViewModel, directory);

            _status.SetStatus(Status.Loaded);
        }

        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;

            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}