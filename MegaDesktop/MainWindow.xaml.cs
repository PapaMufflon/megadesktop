using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MegaApi;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;
using MegaDesktop;
using System.Diagnostics;

namespace MegaWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ITodo, ICanRefresh, ICanSetTitle
    {
        Mega api;
        private readonly MainViewModel _mainViewModel;

        public MainWindow()
        {
            InitializeComponent();

            var dispatcher = new Dispatcher(Dispatcher);
            _mainViewModel = new MainViewModel(this, this, dispatcher, this);
            DataContext = _mainViewModel;

            CheckTos();

            System.Net.ServicePointManager.DefaultConnectionLimit = 50;
        }

        public Mega Api { get { return api; } }

        private static void CheckTos()
        {
            if (MegaDesktop.Properties.Settings.Default.TosAccepted) { return; }
            else
            {
                TermsOfServiceWindow tos = new TermsOfServiceWindow();
                var res = tos.ShowDialog();
                if (!res.Value)
                {
                    Process.GetCurrentProcess().Kill();
                }
                else
                {
                    MegaDesktop.Properties.Settings.Default.TosAccepted = true;
                    MegaDesktop.Properties.Settings.Default.Save();
                }
            }
        }

        public void RefreshCurrentNode()
        {
            Refresh(_mainViewModel.SelectedListNode);
        }

        public void Reload()
        {
            Refresh();
        }

        private void Refresh(NodeViewModel node = null)
        {
            _mainViewModel.Status.SetStatus(Status.Communicating);

            api.GetNodes(nodes =>
            {
                _mainViewModel.Status.SetStatus(Status.Loaded);
                _mainViewModel.RootNode.Update(nodes);

                _mainViewModel.SelectedListNode = node == null
                                                      ? _mainViewModel.RootNode.Children.Single(n => n.HideMe.Type == MegaNodeType.RootFolder)
                                                      : _mainViewModel.RootNode.Descendant(node.Id);
            }, e => _mainViewModel.Status.Error(e));
        }

        void Invoke(Action fn)
        {
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (Delegate)fn);
        }

        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshCurrentNode();
        }

        void CancelTransfer(TransferHandle handle, bool warn = true)
        {
            if (warn && (handle.Status == TransferHandleStatus.Downloading || handle.Status == TransferHandleStatus.Uploading))
            {
                var type = (handle.Status == TransferHandleStatus.Downloading ? "download" : "upload");
                var text = String.Format("Are you sure to cancel the {0} process for {1}?", type, handle.Node.Attributes.Name);
                if (MessageBox.Show(text, "Cancel " + type, MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
            }
            handle.CancelTransfer();
        }

        private void ButtonCancelTransfer_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var handle = button.DataContext as TransferHandle;
            CancelTransfer(handle);
        }

        private void ButtonRemoveTransfer_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var handle = button.DataContext as TransferHandle;
            _mainViewModel.Transfers.Remove(handle);
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
                    String[] files = (String[])e.Data.GetData(DataFormats.FileDrop);

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

        public void AddUploadHandle(TransferHandle h)
        {
            Invoke(() => _mainViewModel.Transfers.Add(h));
            h.PropertyChanged += (s, ev) =>
            {
                h.TransferEnded += (s1, e1) => RefreshCurrentNode();
            };

            _mainViewModel.Status.SetStatus(Status.Loaded);
        }
    }
}