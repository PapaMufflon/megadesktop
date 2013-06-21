using System;
using System.Windows.Input;
using MegaDesktop.Services;

namespace MegaDesktop.Commands
{
    internal class RefreshCommand : ICommand, IToolBarCommand
    {
        private readonly RefreshService _refresh;
        private readonly IDispatcher _dispatcher;
        private readonly MegaApiWrapper _megaApiWrapper;

        public RefreshCommand(RefreshService refresh, IDispatcher dispatcher, MegaApiWrapper megaApiWrapper)
        {
            _refresh = refresh;
            _dispatcher = dispatcher;
            _megaApiWrapper = megaApiWrapper;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _megaApiWrapper.User != null;
        }

        public void Execute(object parameter)
        {
            _refresh.RefreshCurrentNode();
        }

        public virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;

            if (handler != null)
                _dispatcher.InvokeOnUiThread(() => handler(this, EventArgs.Empty));
        }

        public int Position { get { return 4; } }
        public bool Gap { get { return true; } }
        public string ImageSource { get { return "pack://application:,,,/MegaDesktop;component/resources/Refresh.png"; } }
        public string ToolTip { get { return Resource.Refresh; } }
    }
}