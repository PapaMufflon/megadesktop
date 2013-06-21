using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MegaApi;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;

namespace MegaDesktop.Commands
{
    internal class DeleteCommand : ICommand, IToolBarCommand
    {
        private readonly MegaApiWrapper _megaApiWrapper;
        private readonly RefreshService _refresh;
        private readonly StatusViewModel _status;

        public DeleteCommand(MegaApiWrapper megaApiWrapper, StatusViewModel status, RefreshService refresh)
        {
            _megaApiWrapper = megaApiWrapper;
            _status = status;
            _refresh = refresh;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            var node = parameter as NodeViewModel;

            return node != null &&
                   (node.Type == NodeType.File || node.Type == NodeType.Folder);
        }

        public void Execute(object parameter)
        {
            var node = parameter as NodeViewModel;
            var type = node.Type == NodeType.Folder
                ? "folder"
                : "file";

            if (MessageBox.Show(String.Format("Are you sure to delete the {0} {1}?", type, node.Name),
                                "Deleting " + type,
                                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _megaApiWrapper.RemoveNode(node.Id)
                               .ContinueWith(NodeRemoved);
            }
        }

        private void NodeRemoved(Task task)
        {
            if (task.Exception != null)
                _status.Error(task.Exception.InnerExceptions.First() as MegaApiException);
            else
                _refresh.RefreshCurrentNode();
        }

        public virtual void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public int Position { get { return 3; } }
        public bool Gap { get { return false; } }
        public string ImageSource { get { return "pack://application:,,,/MegaDesktop;component/resources/Delete.png"; } }
        public string ToolTip { get { return Resource.Delete; } }
    }
}