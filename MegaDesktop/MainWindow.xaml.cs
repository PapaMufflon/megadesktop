using System;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Input;
using MegaDesktop.Properties;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;
using Ninject;
using Ninject.Extensions.Conventions;

namespace MegaDesktop
{
    public partial class MainWindow : ICanSetTitle
    {
        private readonly Dispatcher _dispatcher;

        public MainWindow()
        {
            InitializeComponent();

            _dispatcher = new Dispatcher(Dispatcher);

            var kernel = new StandardKernel();
            kernel.Bind(x => x.FromThisAssembly()
                              .SelectAllClasses()
                              .BindToSelf()
                              .Configure(y => y.InTransientScope()));
            kernel.Bind(x => x.FromThisAssembly()
                              .SelectAllClasses()
                              .BindAllInterfaces()
                              .Configure(y => y.InTransientScope()));

            kernel.Bind<IUserManagement>().To<UserAccount>();

            kernel.Rebind<IDispatcher>().ToConstant(_dispatcher);
            kernel.Rebind<MegaApiWrapper>().ToSelf().InSingletonScope();
            kernel.Rebind<StatusViewModel>().ToSelf().InSingletonScope();
            kernel.Rebind<NodeManager>().ToSelf().InSingletonScope();
            kernel.Rebind<TransferManager>().ToSelf().InSingletonScope();
            kernel.Rebind<ICanSetTitle>().ToConstant(this);

            kernel.Get<UserAccount>().LoginLastUser()
                  .ContinueWith(x =>
                      {
                          if (x.Exception == null)
                              return;

                          MessageBox.Show("Error while loading account: " + x.Exception);
                          Application.Current.Shutdown();
                      });

            DataContext = kernel.Get<MainViewModel>();

            CheckTos();

            ServicePointManager.DefaultConnectionLimit = 50;
        }

        public void SetTitle(string newTitle)
        {
            _dispatcher.InvokeOnUiThread(() => Title = newTitle);
        }

        private static void CheckTos()
        {
            if (Settings.Default.TosAccepted)
            {
                return;
            }
            else
            {
                var tos = new TermsOfServiceWindow();
                bool? res = tos.ShowDialog();
                if (!res.Value)
                {
                    Process.GetCurrentProcess().Kill();
                }
                else
                {
                    Settings.Default.TosAccepted = true;
                    Settings.Default.Save();
                }
            }
        }

        private void Window_DragEnter_1(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Window_Drop_1(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if ((e.Effects & DragDropEffects.Copy) == DragDropEffects.Copy)
                {
                    var files = (String[]) e.Data.GetData(DataFormats.FileDrop);

                    if (files.Length > 0)
                    {
                        // todo: enable dragdtop again
                        //MegaNode target = null;
                        //lock (nodes)
                        //{
                        //    target = nodes.Where(n => n.Id == currentNode.Id).First();
                        //}
                        //Util.StartThread(() => ScheduleUpload(files, target), "drag_drop_upload_start");
                    }
                }
            }
        }

        //private void ScheduleUpload(string[] files, MegaNode target)
        //{
        //    _mainViewModel.Status.SetStatus(Status.Processing);
        //    var list = new List<MegaApi.Utility.Tuple<string, string>>();
        //    foreach (var file in files)
        //    {
        //        var root = Path.GetDirectoryName(file);
        //        list.Add(new MegaApi.Utility.Tuple<string, string>(file, root));
        //        if ((new FileInfo(file).Attributes & FileAttributes.Directory) == FileAttributes.Directory)
        //        {
        //            AddDirectoryContent(file, list, root);
        //        }
        //    }
        //    foreach (var file in list)
        //    {
        //        var filename = file.Item1.Replace(file.Item2, "").TrimStart(Path.DirectorySeparatorChar);
        //        var folder = Path.GetDirectoryName(filename);
        //        try
        //        {
        //            var d = api.CreateFolderSync(target, nodes, folder, Path.DirectorySeparatorChar);
        //            var fi = new FileInfo(file.Item1);
        //            if ((fi.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
        //            {
        //                try
        //                {
        //                    nodes.Add(api.CreateFolderSync(d.Id, Path.GetFileName(filename)));
        //                }
        //                catch (MegaApiException e)
        //                {
        //                    _mainViewModel.Status.Error(e.ErrorNumber);
        //                }
        //            }
        //            else
        //            {
        //                if (fi.Length > 0)
        //                {
        //                    api.UploadFile(d.Id, file.Item1, AddUploadHandle, e => _mainViewModel.Status.Error(e));
        //                }
        //            }
        //        }
        //        catch (MegaApiException e)
        //        {
        //            _mainViewModel.Status.Error(e.ErrorNumber);
        //        }
        //    }

        //    _mainViewModel.Status.SetStatus(Status.Loaded);
        //}

        //private void AddDirectoryContent(string path, List<MegaApi.Utility.Tuple<string, string>> list, string root)
        //{
        //    foreach (var file in Directory.GetFiles(path))
        //    {
        //        list.Add(new MegaApi.Utility.Tuple<string, string>(file, root));
        //    }
        //    foreach (var subdir in Directory.GetDirectories(path))
        //    {
        //        list.Add(new MegaApi.Utility.Tuple<string, string>(subdir, root));
        //        AddDirectoryContent(subdir, list, root);
        //    }
        //}

        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}