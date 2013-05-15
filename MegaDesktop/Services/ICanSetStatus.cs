using System;

namespace MegaDesktop.Services
{
    internal interface ICanSetStatus
    {
        event EventHandler<EventArgs> CurrentStatusChanged;

        void SetStatus(Status newStatus);
        void Error(int errorNumber);

        Status CurrentStatus { get; }
    }
}