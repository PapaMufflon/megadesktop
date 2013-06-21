using MegaDesktop.Commands;

namespace MegaDesktop.ViewModels
{
    internal class HeaderViewModel
    {
        public HeaderViewModel(StatusViewModel statusViewModel, LoginCommand loginCommand, LogoutCommand logoutCommand, ExitCommand exitCommand)
        {
            Status = statusViewModel;
            LoginCommand = loginCommand;
            LogoutCommand = logoutCommand;
            ExitCommand = exitCommand;
        }

        public StatusViewModel Status { get; private set; }
        public LoginCommand LoginCommand { get; set; }
        public LogoutCommand LogoutCommand { get; set; }
        public ExitCommand ExitCommand { get; set; }
    }
}