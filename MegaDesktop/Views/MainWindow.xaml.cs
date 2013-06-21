using System.Windows.Input;
using MegaDesktop.Services;

namespace MegaDesktop.Views
{
    public partial class MainWindow : ICanSetTitle
    {
        private readonly Dispatcher _dispatcher;

        public MainWindow()
        {
            InitializeComponent();

            _dispatcher = new Dispatcher(Dispatcher);
        }

        public void SetTitle(string newTitle)
        {
            _dispatcher.InvokeOnUiThread(() => Title = newTitle);
        }

        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}