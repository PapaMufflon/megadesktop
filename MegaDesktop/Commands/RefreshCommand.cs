using System;
using System.Windows.Input;
using MegaDesktop.Services;

namespace MegaDesktop.Commands
{
    internal class RefreshCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly ICanRefresh _refresh;
        
        public RefreshCommand(ICanRefresh refresh)
        {
            _refresh = refresh;
        }

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
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}