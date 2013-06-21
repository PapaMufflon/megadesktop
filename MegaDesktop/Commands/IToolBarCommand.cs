namespace MegaDesktop.Commands
{
    internal interface IToolBarCommand
    {
        int Position { get; }
        bool Gap { get; }
        string ImageSource { get; }
        string ToolTip { get; }
    }
}