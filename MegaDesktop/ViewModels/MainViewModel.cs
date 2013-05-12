using System.Collections.ObjectModel;
using System.Windows.Input;
using MegaDesktop.Commands;
using MegaDesktop.Services;
using MegaWpf;

namespace MegaDesktop.ViewModels
{
    internal class MainViewModel
    {
        public MainViewModel(ITodo todo, IDispatcher dispatcher)
        {
            Status = new StatusViewModel { Message = "Retrieving the list of files..." };
            UploadCommand = new UploadCommand(todo.Api, Status, todo, dispatcher);
            RootNode = new NodeViewModel(null, dispatcher);
        }

        public ICanSetStatus Status { get; set; }
        public ICommand UploadCommand { get; set; }
        public NodeViewModel RootNode { get; private set; }
        public NodeViewModel SelectedTreeNode { get; set; }
        public NodeViewModel SelectedListNode { get; set; }
    }
}