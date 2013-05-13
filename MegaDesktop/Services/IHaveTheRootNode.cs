using MegaDesktop.ViewModels;

namespace MegaDesktop.Services
{
    internal interface IHaveTheRootNode
    {
        NodeViewModel RootNode { get; }
    }
}