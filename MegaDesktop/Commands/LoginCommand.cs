using System;
using System.Windows.Input;
using MegaDesktop.Services;

namespace MegaDesktop.Commands
{
    internal class LoginCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly IMegaApi _megaApi;
        private readonly IUserManagement _userAccount;
        private readonly IManageTransfers _transfers;
        private readonly ICanSetTitle _title;
        private readonly ICanRefresh _refresh;

        public LoginCommand(IMegaApi megaApi, IUserManagement userAccount, IManageTransfers transfers, ICanSetTitle title, ICanRefresh refresh)
        {
            _megaApi = megaApi;
            _userAccount = userAccount;
            _transfers = transfers;
            _title = title;
            _refresh = refresh;
        }

        public bool CanExecute(object parameter)
        {
            return _megaApi.User == null;
        }

        public void Execute(object parameter)
        {
            var w = new WindowLogin();
            w.OnLoggedIn += (s, args) =>
            {
                _megaApi.Use(args.Api);
                _userAccount.SaveCurrentAccount();
                _transfers.CancelAllTransfers();
                w.Close();
                _title.SetTitle(Resource.Title + " - " + _megaApi.User.Email);
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