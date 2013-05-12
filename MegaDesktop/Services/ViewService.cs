using System;
using System.Windows.Threading;

namespace MegaDesktop.Services
{
    internal class ViewService : IViewService
    {
        private readonly Dispatcher _dispatcher;

        public ViewService(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void InvokeOnUiThread(Action action)
        {
            _dispatcher.Invoke(action);
        }
    }
}