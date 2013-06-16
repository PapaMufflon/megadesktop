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
    internal class DeleteCommand : ICommand
    {
        private readonly MegaApiWrapper _megaApiWrapper;
        private readonly RefreshService _refresh;
        private readonly StatusViewModel _status;

        public DeleteCommand(MegaApiWrapper megaApiWrapper, StatusViewModel status, RefreshService refresh,
                             NodeManager nodes, IDispatcher dispatcher)
        {
            _megaApiWrapper = megaApiWrapper;
            _status = status;
            _refresh = refresh;

            nodes.SelectedNodeChanged += (s, e) => dispatcher.InvokeOnUiThread(OnCanExecuteChanged);
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
            var node = (parameter as NodeViewModel);
            string type = (node.Type == NodeType.Folder ? "folder" : "file");
            string text = String.Format("Are you sure to delete the {0} {1}?", type, node.Name);
            if (MessageBox.Show(text, "Deleting " + type, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _megaApiWrapper.RemoveNode(node.Id)
                               .ContinueWith(x =>
                                   {
                                       if (x.Exception != null)
                                           _status.Error(x.Exception.InnerExceptions.First() as MegaApiException);
                                       else
                                           _refresh.RefreshCurrentNode();
                                   });
            }
        }

        protected virtual void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}