namespace MegaDesktop.Commands
{
    internal interface ICanSetStatus
    {
        void Set(string newStatus);
        void Done();
        void Error(int errorNumber);
    }
}