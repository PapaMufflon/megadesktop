using System;
using System.Windows.Input;
using MegaApi;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;

namespace MegaDesktop.Commands
{
    internal class SelectedListNodeActionCommand : ICommand
    {
        private readonly DownloadCommand _downloadCommand;
        private readonly ICanRefresh _refresh;

        public SelectedListNodeActionCommand(DownloadCommand downloadCommand, ICanRefresh refresh)
        {
            _downloadCommand = downloadCommand;
            _refresh = refresh;
        }

        public bool CanExecute(object parameter)
        {
            return _downloadCommand.CanExecute(parameter);
        }

        public void Execute(object parameter)
        {
            var selectedNode = parameter as NodeViewModel;

            if (selectedNode.Type == NodeType.Dummy ||
                selectedNode.Type == NodeType.Folder)
            {
                _refresh.RefreshCurrentNode();
            }

            if (selectedNode.Type == NodeType.File)
            {
                _downloadCommand.Execute(parameter);
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}