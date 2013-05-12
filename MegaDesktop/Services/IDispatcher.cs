using System;

namespace MegaDesktop.Services
{
    internal interface IDispatcher
    {
        void InvokeOnUiThread(Action action);
    }
}