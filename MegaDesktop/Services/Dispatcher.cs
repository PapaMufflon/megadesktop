using System;
using MegaDesktop.Util;

namespace MegaDesktop.Services
{
    internal class Dispatcher : IDispatcher
    {
        private readonly System.Windows.Threading.Dispatcher _dispatcher;

        public Dispatcher(System.Windows.Threading.Dispatcher dispatcher)
        {
            _dispatcher = dispatcher.AssertIsNotNull("dispatcher");
        }

        public void InvokeOnUiThread(Action action)
        {
            _dispatcher.Invoke(action);
        }
    }
}