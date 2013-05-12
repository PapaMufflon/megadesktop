using System;

namespace MegaDesktop.Commands
{
    internal interface ICanSetStatus
    {
        event EventHandler<EventArgs> CurrentStatusChanged;

        void SetStatus(Status newStatus);
        void Error(int errorNumber);

        Status CurrentStatus { get; }
    }

    internal enum Status
    {
        LoggingIn,
        Communicating,
        Loaded,
        Processing
    }
}