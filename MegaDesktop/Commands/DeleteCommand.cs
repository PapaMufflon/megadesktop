using System;
using System.Windows;
using System.Windows.Input;
using MegaApi;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;

namespace MegaDesktop.Commands
{
    internal class DeleteCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly IMegaApi _megaApi;
        private readonly ICanSetStatus _status;
        private readonly ICanRefresh _refresh;

        public DeleteCommand(IMegaApi megaApi, ICanSetStatus status, ICanRefresh refresh, ISelectedNodeListener listener, IDispatcher dispatcher)
        {
            _megaApi = megaApi;
            _status = status;
            _refresh = refresh;

            listener.SelectedNodeChanged += (s, e) => dispatcher.InvokeOnUiThread(OnCanExecuteChanged);
        }

        public bool CanExecute(object parameter)
        {
            var node = parameter as NodeViewModel;

            return node != null &&
                   (node.Type == NodeType.File || node.Type == NodeType.Folder);
        }

        public void Execute(object parameter)
        {
            var node = (parameter as NodeViewModel);
            var type = (node.Type == NodeType.Folder ? "folder" : "file");
            var text = String.Format("Are you sure to delete the {0} {1}?", type, node.Name);
            if (MessageBox.Show(text, "Deleting " + type, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _megaApi.RemoveNode(node.Id, () => _refresh.RefreshCurrentNode(), err => _status.Error(err));
            }
        }

        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}