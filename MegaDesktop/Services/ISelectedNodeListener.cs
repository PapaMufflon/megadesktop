using System;

namespace MegaDesktop.Services
{
    internal interface ISelectedNodeListener
    {
        event EventHandler<EventArgs> SelectedNodeChanged;
    }
}