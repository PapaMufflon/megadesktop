using System;
using System.Windows.Input;
using MegaDesktop.Services;

namespace MegaDesktop.Commands
{
    internal class RefreshCommand : ICommand
    {
        private readonly RefreshService _refresh;

        public RefreshCommand(RefreshService refresh)
        {
            _refresh = refresh;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _refresh.RefreshCurrentNode();
        }

        protected virtual void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}