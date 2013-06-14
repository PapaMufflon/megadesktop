using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using MegaApi;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;

namespace MegaDesktop.Commands
{
    internal class UploadFolderCommand : ICommand
    {
        private readonly UploadCommand _uploadCommand;
        private readonly MegaApiWrapper _megaApiWrapper;
        private readonly IDispatcher _dispatcher;
        private readonly NodeManager _nodes;
        private readonly StatusViewModel _status;

        public UploadFolderCommand(StatusViewModel status, NodeManager nodes,
                                   IDispatcher dispatcher, MegaApiWrapper megaApiWrapper,
            UploadCommand uploadCommand)
        {
            _status = status.AssertIsNotNull("status");
            _nodes = nodes.AssertIsNotNull("nodes");
            _dispatcher = dispatcher.AssertIsNotNull("dispatcher");
            _megaApiWrapper = megaApiWrapper.AssertIsNotNull("megaApiWrapper");
            _uploadCommand = uploadCommand.AssertIsNotNull("uploadCommand");

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
            DialogResult result = d.ShowDialog();

            if (result != DialogResult.OK)
                return;

            CreateFolder(currentNode, d.SelectedPath);
        }

        private void CreateFolder(NodeViewModel node, string path)
        {
            _status.SetStatus(Status.Communicating);
            _megaApiWrapper.CreateFolder(node.Id, Path.GetFileName(path), x => OnSuccess(x, path), err => _status.Error(err));
        }

        private void OnSuccess(MegaNode node, string path)
        {
            var nodeViewModel = new NodeViewModel(_dispatcher, node);

            NodeViewModel parent = _nodes.RootNode.Descendant(node.ParentId);

            _dispatcher.InvokeOnUiThread(() =>
                {
                    parent.Children.Add(nodeViewModel);
                    parent.ChildNodes.Add(nodeViewModel);
                });

            foreach (var file in Directory.EnumerateFiles(path))
                _uploadCommand.UploadFile(file, nodeViewModel);

            foreach (var directory in Directory.EnumerateDirectories(path))
                CreateFolder(nodeViewModel, directory);

            _status.SetStatus(Status.Loaded);
        }

        protected virtual void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}