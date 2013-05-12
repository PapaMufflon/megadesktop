using System;
using System.Windows.Input;
using MegaDesktop.Services;
using MegaWpf;

namespace MegaDesktop.Commands
{
    internal class LoginCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly IHaveTheApi _apiManager;
        private readonly IUserManagement _userAccount;
        private readonly IManageTransfers _transfers;
        private readonly ICanSetTitle _title;
        private readonly ICanRefresh _refresh;

        public LoginCommand(IHaveTheApi apiManager, IUserManagement userAccount, IManageTransfers transfers, ICanSetTitle title, ICanRefresh refresh)
        {
            _apiManager = apiManager;
            _userAccount = userAccount;
            _transfers = transfers;
            _title = title;
            _refresh = refresh;
        }

        public bool CanExecute(object parameter)
        {
            return _apiManager.Api.User == null;
        }

        public void Execute(object parameter)
        {
            var w = new WindowLogin();
            w.OnLoggedIn += (s, args) =>
            {
                _apiManager.Set(args.Api);
                _userAccount.SaveAccount();
                _transfers.CancelAllTransfers();
                w.Close();
                _title.Title = Resource.Title + " - " + _apiManager.Api.User.Email;
                _refresh.Reload();
            };
            w.ShowDialog();
        }

        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}