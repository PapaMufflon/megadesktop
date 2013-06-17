using System;
using System.Windows.Input;
using MegaDesktop.Services;

namespace MegaDesktop.Commands
{
    internal class LoginCommand : ICommand
    {
        private readonly MegaApiWrapper _megaApiWrapper;
        private readonly IUserManagement _userAccount;

        public LoginCommand(MegaApiWrapper megaApiWrapper, IUserManagement userAccount, IDispatcher dispatcher)
        {
            _megaApiWrapper = megaApiWrapper;
            _userAccount = userAccount;

            _megaApiWrapper.ApiChanged += (s, e) => dispatcher.InvokeOnUiThread(OnCanExecuteChanged);
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _megaApiWrapper.User == null;
        }

        public void Execute(object parameter)
        {
            var w = new WindowLogin(_userAccount);
            w.ShowDialog();
        }

        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;

            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}