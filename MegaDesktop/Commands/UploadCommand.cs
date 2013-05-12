using System;
using System.Windows.Input;
using MegaApi;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;
using MegaWpf;
using Microsoft.Win32;

namespace MegaDesktop.Commands
{
    internal class UploadCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly Mega _api;
        private readonly ICanSetStatus _status;
        private readonly ITodo _todo;
        private readonly IDispatcher _dispatcher;

        public UploadCommand(Mega api, ICanSetStatus status, ISelectedNodeListener selectedNodeListener, ITodo todo, IDispatcher dispatcher)
        {
            _api = api;
            _status = status;
            _todo = todo;
            _dispatcher = dispatcher;

            _status.CurrentStatusChanged += (s, e) => _dispatcher.InvokeOnUiThread(OnCanExecuteChanged);
            selectedNodeListener.SelectedNodeChanged += (s, e) => dispatcher.InvokeOnUiThread(OnCanExecuteChanged);
        }

        public void Execute(object parameter)
        {
            var currentNode = parameter as NodeViewModel;

            if (currentNode == null)
                return;

            var d = new OpenFileDialog();

            if (d.ShowDialog() != true)
                return;

            _status.SetStatus(Status.Communicating);
            _api.UploadFile(currentNode.Id, d.FileName, _todo.AddUploadHandle, err => _status.Error(err));
        }

        public bool CanExecute(object parameter)
        {
            var node = parameter as NodeViewModel;

            return node != null &&
                   node.HideMe.Type != MegaNodeType.File &&
                   _status.CurrentStatus == Status.Loaded;
        }

        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
