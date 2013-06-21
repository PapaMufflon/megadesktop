using System;
using System.Linq;
using System.Windows.Input;
using MegaDesktop.Services;

namespace MegaDesktop.Commands
{
    internal class LogoutCommand : ICommand
    {
        private readonly NodeManager _nodes;
        private readonly TransferManager _transfers;
        private readonly IUserManagement _userAccount;
        private readonly MegaApiWrapper _apiWrapper;

        public LogoutCommand(TransferManager transfers,
                             NodeManager nodes,
                             IUserManagement userAccount,
                             IDispatcher dispatcher,
                             MegaApiWrapper apiWrapper)
        {
            _transfers = transfers;
            _nodes = nodes;
            _userAccount = userAccount;
            _apiWrapper = apiWrapper;

            _nodes.RootNode.Children.CollectionChanged += (s, e) => dispatcher.InvokeOnUiThread(OnCanExecuteChanged);
            _apiWrapper.ApiChanged += (s, e) => dispatcher.InvokeOnUiThread(OnCanExecuteChanged);
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _apiWrapper.User != null;
        }

        public void Execute(object parameter)
        {
            _transfers.CancelAllTransfers();
            _transfers.Transfers.Clear();

            while (_nodes.RootNode.Children.Any())
                _nodes.RootNode.Children.RemoveAt(_nodes.RootNode.Children.Count - 1);

            _userAccount.DeleteCurrentAccount();
            _apiWrapper.Register(null);
        }

        protected virtual void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}