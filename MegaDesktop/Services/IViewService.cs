using System;

namespace MegaDesktop.Services
{
    internal interface IViewService
    {
        void InvokeOnUiThread(Action action);
    }
}