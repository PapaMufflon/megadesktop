using System;
using System.IO;
using System.Windows.Input;

namespace MegaDesktop.Commands
{
    internal class LogoutCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        
        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            _mainViewModel.CancelAllTransfers();
            Invoke(() =>
            {
                _mainViewModel.Transfers.Clear();

                while (_mainViewModel.RootNode.Children.Any())
                    _mainViewModel.RootNode.Children.RemoveAt(_mainViewModel.RootNode.Children.Count - 1);
            });
            var userAccount = GetUserKeyFilePath();
            // to restore previous anon account
            //File.Move(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(userAccount), "user.anon.dat"), userAccount);
            // or simply drop logged in account
            File.Delete(userAccount);
            Login(false, userAccount);
        }

        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}