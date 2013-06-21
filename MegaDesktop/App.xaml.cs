using System.Net;
using System.Threading.Tasks;
using System.Windows;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;
using MegaDesktop.Views;
using Ninject;
using Ninject.Extensions.Conventions;

namespace MegaDesktop
{
    public partial class App
    {
        private StandardKernel _kernel;
        private MainWindow _mainView;

        private void ApplicationStart(object sender, StartupEventArgs e)
        {
            ServicePointManager.DefaultConnectionLimit = 50;

            CreateBindings();
            LogIn();
            ShowMainView();
        }

        private void CreateBindings()
        {
            _kernel = new StandardKernel();

            _kernel.Bind(x => x.FromThisAssembly()
                              .SelectAllClasses()
                              .BindToSelf()
                              .Configure(y => y.InSingletonScope()));
            _kernel.Bind(x => x.FromThisAssembly().IncludingNonePublicTypes()
                              .SelectAllClasses()
                              .BindAllInterfaces()
                              .Configure(y => y.InSingletonScope()));

            _kernel.Rebind<IDispatcher>().ToConstant(new Dispatcher(Dispatcher));
            _kernel.Rebind<MegaApiWrapper>().ToSelf().InSingletonScope();
            _kernel.Rebind<StatusViewModel>().ToSelf().InSingletonScope();
            _kernel.Rebind<NodeManager>().ToSelf().InSingletonScope();
            _kernel.Rebind<TransferManager>().ToSelf().InSingletonScope();
        }

        private void LogIn()
        {
            _kernel.Get<UserAccount>()
                   .LoginLastUser()
                   .ContinueWith(AssertLoggedIn);
        }

        private void AssertLoggedIn(Task task)
        {
            if (task.Exception == null)
                return;

            MessageBox.Show("Error while loading account: " + task.Exception);
            Current.Shutdown();
        }

        private void ShowMainView()
        {
            _mainView = _kernel.Get<MainWindow>();
            _kernel.Rebind<ICanSetTitle>().ToConstant(_mainView);

            _mainView.DataContext = _kernel.Get<MainViewModel>();

            _mainView.Show();
        }
    }
}