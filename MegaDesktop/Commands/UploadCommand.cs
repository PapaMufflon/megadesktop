using System;
using System.Windows.Input;
using MegaApi;
using MegaDesktop.Services;
using MegaWpf;
using Microsoft.Win32;

namespace MegaDesktop.Commands
{
    internal class UploadCommand : ICommand
    {
        private readonly Mega _api;
        private readonly ICanSetStatus _status;
        private readonly ITodo _todo;
        private readonly IDispatcher _dispatcher;

        public UploadCommand(Mega api, ICanSetStatus status, ITodo todo, IDispatcher dispatcher)
        {
            _api = api;
            _status = status;
            _todo = todo;
            _dispatcher = dispatcher;

            _status.CurrentStatusChanged += (s, e) => _dispatcher.InvokeOnUiThread(OnCanExecuteChanged);
        }

        public void Execute(object parameter)
        {
            var currentNode = parameter as MegaNode;

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
            return _status.CurrentStatus == Status.Loaded;
        }

        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
