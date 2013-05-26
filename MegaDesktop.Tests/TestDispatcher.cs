using System;
using MegaDesktop.Services;

namespace MegaDesktop.Tests
{
    public class TestDispatcher : IDispatcher
    {
        public void InvokeOnUiThread(Action action)
        {
            action.Invoke();
        }
    }
}