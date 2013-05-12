using System.Windows.Input;
using MegaDesktop.Commands;
using MegaDesktop.Services;
using MegaWpf;

namespace MegaDesktop.ViewModels
{
    internal class MainViewModel
    {
        public MainViewModel(ITodo todo, IViewService viewService)
        {
            Status = new StatusViewModel { Message = "Retrieving the list of files..." };
            UploadCommand = new UploadCommand(todo.Api, Status, todo, viewService);
        }

        public ICanSetStatus Status { get; set; }
        public ICommand UploadCommand { get; set; }
    }
}