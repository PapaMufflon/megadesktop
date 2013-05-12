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
        
        private readonly Mega _api;
        private readonly ICanSetStatus _status;
        private readonly ICanRefresh _refresh;

        public DeleteCommand(Mega api, ICanSetStatus status, ICanRefresh refresh)
        {
            _api = api;
            _status = status;
            _refresh = refresh;
        }

        public bool CanExecute(object parameter)
        {
            var node = parameter as NodeViewModel;

            return node != null &&
                   node.HideMe != null &&
                   (node.HideMe.Type == MegaNodeType.File || node.HideMe.Type == MegaNodeType.Folder);
        }

        public void Execute(object parameter)
        {
            var node = (parameter as NodeViewModel).HideMe;
            var type = (node.Type == MegaNodeType.Folder ? "folder" : "file");
            var text = String.Format("Are you sure to delete the {0} {1}?", type, node.Attributes.Name);
            if (MessageBox.Show(text, "Deleting " + type, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _api.RemoveNode(node.Id, () => _refresh.RefreshCurrentNode(), err => _status.Error(err));
            }
        }

        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
