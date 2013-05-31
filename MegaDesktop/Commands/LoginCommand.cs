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

        public LoginCommand(IMegaApi megaApi, IUserManagement userAccount)
        {
            _megaApi = megaApi;
            _userAccount = userAccount;
        }

        public bool CanExecute(object parameter)
        {
            return _megaApi.User == null;
        }

        public void Execute(object parameter)
        {
            var w = new WindowLogin(_userAccount);
            w.ShowDialog();
        }

        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}