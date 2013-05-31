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
        public event EventHandler CanExecuteChanged;
        
        private readonly IMegaApi _megaApi;
        private readonly ICanSetStatus _status;
        private readonly IHaveNodes _nodes;
        private readonly IDispatcher _dispatcher;

        public UploadFolderCommand(IMegaApi megaApi, ICanSetStatus status, ISelectedNodeListener selectedNodeListener, IHaveNodes nodes, IDispatcher dispatcher)
        {
            _megaApi = megaApi.AssertIsNotNull("megaApi");
            _status = status.AssertIsNotNull("status");
            _nodes = nodes.AssertIsNotNull("nodes");
            _dispatcher = dispatcher.AssertIsNotNull("dispatcher");

            selectedNodeListener.AssertIsNotNull("selectedNodeListener").SelectedNodeChanged += (s, e) => dispatcher.InvokeOnUiThread(OnCanExecuteChanged);
        }

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
            var result = d.ShowDialog();

            if (result != DialogResult.OK)
                return;

            _status.SetStatus(Status.Communicating);
            _megaApi.CreateFolder(currentNode.Id, Path.GetFileName(d.SelectedPath), OnSuccess, err => _status.Error(err));
        }

        private void OnSuccess(MegaNode node)
        {
            var nodeViewModel = new NodeViewModel(_dispatcher, node);

            var parent = _nodes.RootNode.Descendant(node.ParentId);

            _dispatcher.InvokeOnUiThread(() =>
            {
                parent.Children.Add(nodeViewModel);
                parent.ChildNodes.Add(nodeViewModel);        
            });
        }

        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}