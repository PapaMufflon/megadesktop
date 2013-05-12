using System;
using System.Windows.Input;
using MegaApi;
using MegaWpf;
using Microsoft.Win32;

namespace MegaDesktop.Commands
{
    internal class UploadCommand : ICommand
    {
        private readonly Mega _api;
        private readonly ICanSetStatus _status;
        private readonly ITodo _todo;

        public UploadCommand(Mega api, ICanSetStatus status, ITodo todo)
        {
            _api = api;
            _status = status;
            _todo = todo;
        }

        public void Execute(object parameter)
        {
            var currentNode = parameter as MegaNode;

            if (currentNode == null)
                return;

            var d = new OpenFileDialog();

            if (d.ShowDialog() != true)
                return;

            _status.Set("Starting upload...");
            _api.UploadFile(currentNode.Id, d.FileName, _todo.AddUploadHandle, err => _status.Error(err));
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}
