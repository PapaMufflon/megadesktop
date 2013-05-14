using System;
using System.IO;
using System.Linq;
using System.Windows.Input;
using MegaDesktop.Services;

namespace MegaDesktop.Commands
{
    internal class LogoutCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly IManageTransfers _transfers;
        private readonly IHaveTheRootNode _rootNode;
        private readonly IUserManagement _userAccount;

        public LogoutCommand(IManageTransfers transfers, IHaveTheRootNode rootNode, IUserManagement userAccount)
        {
            _transfers = transfers;
            _rootNode = rootNode;
            _userAccount = userAccount;
        }

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
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}