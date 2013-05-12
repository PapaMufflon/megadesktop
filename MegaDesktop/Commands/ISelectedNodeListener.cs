using System;

namespace MegaDesktop.Commands
{
    internal interface ISelectedNodeListener
    {
        event EventHandler<EventArgs> SelectedNodeChanged;
    }
}