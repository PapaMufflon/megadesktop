using System;
using System.Linq;
using System.Windows.Input;
using MegaDesktop.Services;

namespace MegaDesktop.Commands
{
    internal class LogoutCommand : ICommand
    {
        private readonly NodeManager _rootNode;
        private readonly TransferManager _transfers;
        private readonly IUserManagement _userAccount;

        public LogoutCommand(TransferManager transfers, NodeManager rootNode, IUserManagement userAccount)
        {
            _transfers = transfers;
            _rootNode = rootNode;
            _userAccount = userAccount;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _rootNode.RootNode.Children.Any();
        }

        public void Execute(object parameter)
        {
            _transfers.CancelAllTransfers();
            _transfers.Transfers.Clear();

            while (_rootNode.RootNode.Children.Any())
                _rootNode.RootNode.Children.RemoveAt(_rootNode.RootNode.Children.Count - 1);

            _userAccount.DeleteCurrentAccount();
        }

        protected virtual void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}