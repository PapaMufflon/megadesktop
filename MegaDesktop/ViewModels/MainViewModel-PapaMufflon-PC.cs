namespace MegaDesktop.ViewModels
{
    internal class MainViewModel
    {
        public MainViewModel()
        {
            StatusViewModel = new StatusViewModel { Message = "Retrieving the list of files..." };
        }

        public StatusViewModel StatusViewModel { get; set; }
    }
}