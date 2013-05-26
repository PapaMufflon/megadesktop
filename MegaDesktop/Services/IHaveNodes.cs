using MegaDesktop.ViewModels;

namespace MegaDesktop.Services
{
    internal interface IHaveNodes
    {
        NodeViewModel RootNode { get; }
        NodeViewModel SelectedListNode { get; set; }
    }
}