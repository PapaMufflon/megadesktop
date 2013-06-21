using System;
using System.Windows.Input;
using MegaDesktop.Services;

namespace MegaDesktop.Commands
{
    internal class RefreshCommand : ICommand, IToolBarCommand
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

        public virtual void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public int Position { get { return 4; } }
        public bool Gap { get { return true; } }
        public string ImageSource { get { return "pack://application:,,,/MegaDesktop;component/resources/Refresh.png"; } }
        public string ToolTip { get { return Resource.Refresh; } }
    }
}