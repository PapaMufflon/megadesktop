using System;
using System.Windows.Input;
using MegaApi;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;
using Microsoft.Win32;

namespace MegaDesktop.Commands
{
    internal class DownloadCommand : ICommand
    {
        private readonly Mega _api;
        private readonly ICanSetStatus _status;
        private readonly ITodo _todo;

        public DownloadCommand(Mega api, ICanSetStatus status, ITodo todo, ISelectedNodeListener selectedNodeListener, IDispatcher dispatcher)
        {
            _api = api;
            _status = status;
            _todo = todo;

            selectedNodeListener.SelectedNodeChanged += (s, e) => dispatcher.InvokeOnUiThread(OnCanExecuteChanged);
        }

        public bool CanExecute(object parameter)
        {
            var node = parameter as NodeViewModel;

            return node != null &&
                   node.HideMe.Type == MegaNodeType.File;
        }

        public void Execute(object parameter)
        {
            var node = (parameter as NodeViewModel).HideMe;

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
            var d = new SaveFileDialog();
            d.FileName = clickedNode.Attributes.Name;
            if (d.ShowDialog() == true)
            {
                _status.SetStatus(Status.Communicating);
                _api.DownloadFile(clickedNode, d.FileName, _todo.AddUploadHandle, e => _status.Error(e));
            }
        }

        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
