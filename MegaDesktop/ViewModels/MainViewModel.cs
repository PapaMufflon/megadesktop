using System.Windows.Input;
using MegaDesktop.Commands;
using MegaWpf;

namespace MegaDesktop.ViewModels
{
    internal class MainViewModel
    {
        public MainViewModel(ITodo todo)
        {
            Status = new StatusViewModel { Message = "Retrieving the list of files..." };
            UploadCommand = new UploadCommand(todo.Api, Status, todo);
        }

        public ICanSetStatus Status { get; set; }
        public ICommand UploadCommand { get; set; }
    }
}