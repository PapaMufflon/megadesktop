using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using MegaApi;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;
using Ninject;

namespace MegaDesktop.Commands
{
    internal class UploadFolderCommand : ICommand
    {
        private readonly MegaApiWrapper _megaApiWrapper;
        private readonly IDispatcher _dispatcher;
        private readonly NodeManager _nodes;
        private readonly StatusViewModel _status;

        public UploadFolderCommand(StatusViewModel status, NodeManager nodes,
                                   IDispatcher dispatcher, MegaApiWrapper megaApiWrapper)
        {
            _status = status.AssertIsNotNull("status");
            _nodes = nodes.AssertIsNotNull("nodes");
            _dispatcher = dispatcher.AssertIsNotNull("dispatcher");
            _megaApiWrapper = megaApiWrapper.AssertIsNotNull("megaApiWrapper");

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

            if (currentNode == null)
                return;

            var d = new FolderBrowserDialog();
            DialogResult result = d.ShowDialog();

            if (result != DialogResult.OK)
                return;

            _status.SetStatus(Status.Communicating);
            _megaApiWrapper.CreateFolder(currentNode.Id, Path.GetFileName(d.SelectedPath), OnSuccess, err => _status.Error(err));
        }

        private void OnSuccess(MegaNode node)
        {
            var nodeViewModel = new NodeViewModel(_dispatcher, node);

            NodeViewModel parent = _nodes.RootNode.Descendant(node.ParentId);

            _dispatcher.InvokeOnUiThread(() =>
                {
                    parent.Children.Add(nodeViewModel);
                    parent.ChildNodes.Add(nodeViewModel);
                });
        }

        protected virtual void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}