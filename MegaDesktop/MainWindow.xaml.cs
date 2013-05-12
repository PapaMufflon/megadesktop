﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using MegaApi;
using MegaApi.Utility;
using MegaDesktop.Commands;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using MegaApi.DataTypes;
using MegaDesktop;
using System.Diagnostics;
using System.Reflection;
using MegaApi.Comms;

namespace MegaWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ITodo
    {
        Mega api;
        ObservableCollection<TransferHandle> transfers = new ObservableCollection<TransferHandle>();
        MegaNode currentNode;
        static string title = "Mega Desktop (beta)";
        private readonly MainViewModel _mainViewModel;

        public MainWindow()
        {
            var viewService = new Dispatcher(Dispatcher);
            _mainViewModel = new MainViewModel(this, viewService);
            DataContext = _mainViewModel;

            CheckTos();

            System.Net.ServicePointManager.DefaultConnectionLimit = 50;
            var save = false;
            var userAccountFile = GetUserKeyFilePath();
            Login(save, userAccountFile);
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

        private MegaUser Login(bool save, string userAccountFile)
        {
            MegaUser u;
            if ((u = Mega.LoadAccount(userAccountFile)) == null) { save = true; }
            _mainViewModel.Status.SetStatus(Status.LoggingIn);
            Mega.Init(u, (m) =>
            {
                api = m;
                if (save) { SaveAccount(userAccountFile, "user.anon.dat"); }
                InitializeComponent();
                InitialLoadNodes();
                _mainViewModel.Status.SetStatus(Status.Loaded);
                if (api.User.Status == MegaUserStatus.Anonymous)
                {
                    Invoke(() =>
                        {
                            buttonLogin.Visibility = System.Windows.Visibility.Visible;
                            buttonLogout.Visibility = System.Windows.Visibility.Collapsed;
                            Title = title + " - anonymous account";
                        });
                }
                else
                {
                    Invoke(() =>
                        {
                            Title = title + " - " + m.User.Email;
                            buttonLogin.Visibility = System.Windows.Visibility.Collapsed;
                            buttonLogout.Visibility = System.Windows.Visibility.Visible;
                        });
                }
            }, (e) => { MessageBox.Show("Error while loading account: " + e); Application.Current.Shutdown(); });
            return u;
        }

        private void SaveAccount(string userAccountFile, string backupFileName)
        {
            if (File.Exists(userAccountFile))
            {
                var backupFile = System.IO.Path.GetDirectoryName(userAccountFile) +
                                System.IO.Path.DirectorySeparatorChar +
                                backupFileName;
                File.Copy(userAccountFile, backupFile, true);
            }
            api.SaveAccount(GetUserKeyFilePath());
        }

        private void InitialLoadNodes()
        {
            Invoke(() => listBoxDownloads.ItemsSource = transfers);
            _mainViewModel.Status.SetStatus(Status.Communicating);
            api.GetNodes((list) =>
            {
                Invoke(() =>
                {
                    buttonRefresh.IsEnabled = true;
                    buttonLogout.IsEnabled = true;
                    buttonLogin.IsEnabled = true;
                });
                _mainViewModel.Status.SetStatus(Status.Loaded);
                _mainViewModel.RootNode.Update(list);
                currentNode = list.Where(n => n.Type == MegaNodeType.RootFolder).FirstOrDefault();
                ShowFiles();
            }, e => _mainViewModel.Status.Error(e));
        }

        private void ShowFiles(MegaNode parent = null, bool refresh = false)
        {
            if (parent == null) { parent = currentNode; }
            if (refresh)
            {
                _mainViewModel.Status.SetStatus(Status.Communicating);
                api.GetNodes((l) =>
                {
                    _mainViewModel.Status.SetStatus(Status.Loaded);
                    _mainViewModel.RootNode.Update(l);
                    ShowFiles(parent);
                }, e => _mainViewModel.Status.Error(e));
                return;
            }

            var parentViewModel = _mainViewModel.RootNode.Descendant(parent.Id);
            currentNode = parent.Type == MegaNodeType.Dummy ?
                parentViewModel.HideMe : parent;
        }
        void Invoke(Action fn)
        {
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, (Delegate)fn);
        }

        string GetUserKeyFilePath()
        {
            string userDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            userDir += System.IO.Path.DirectorySeparatorChar + "MegaDesktop";
            if (!Directory.Exists(userDir)) { Directory.CreateDirectory(userDir); }
            userDir += System.IO.Path.DirectorySeparatorChar;
            return userDir + "user.dat";
        }

        private void listBoxNodes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var clickedNode = _mainViewModel.SelectedListNode.HideMe;
                if (clickedNode.Type == MegaNodeType.Dummy || clickedNode.Type == MegaNodeType.Folder)
                {
                    ShowFiles(clickedNode);
                }
                if (clickedNode.Type == MegaNodeType.File)
                {
                    DownloadFile(clickedNode);
                }
            }
            catch (InvalidCastException) { return; }

        }

        private void DownloadFile(MegaNode clickedNode)
        {
            var d = new SaveFileDialog();
            d.FileName = clickedNode.Attributes.Name;
            if (d.ShowDialog() == true)
            {
                _mainViewModel.Status.SetStatus(Status.Communicating);
                api.DownloadFile(clickedNode, d.FileName, AddDownloadHandle, e => _mainViewModel.Status.Error(e));
            }
        }

        private void listBoxNodes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_mainViewModel.SelectedListNode != null)
            {
                var node = _mainViewModel.SelectedListNode.HideMe;
                if (node.Type == MegaNodeType.File)
                {
                    buttonDelete.IsEnabled = true;
                }
                else
                {
                    buttonDelete.IsEnabled = node.Type == MegaNodeType.Folder;
                }
            }
            else
            {
                buttonDelete.IsEnabled = false;
            }
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            var node = _mainViewModel.SelectedListNode.HideMe;
            var type = (node.Type == MegaNodeType.Folder ? "folder" : "file");
            var text = String.Format("Are you sure to delete the {0} {1}?", type, node.Attributes.Name);
            if (MessageBox.Show(text, "Deleting " + type, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                api.RemoveNode(node.Id, () => ShowFiles(currentNode, true), err => _mainViewModel.Status.Error(err));
            }
        }

        private void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            var w = new WindowLogin();
            w.OnLoggedIn += (s, args) =>
            {
                api = args.Api;
                SaveAccount(GetUserKeyFilePath(), "user.anon.dat");
                Invoke(() =>
                    {
                        CancelTransfers();
                        w.Close();
                        buttonLogin.Visibility = System.Windows.Visibility.Collapsed;
                        buttonLogout.Visibility = System.Windows.Visibility.Visible;
                        Title = title + " - " + api.User.Email;
                    });
                InitialLoadNodes();
            };
            w.ShowDialog();
        }

        private void CancelTransfers()
        {
            lock (transfers)
            {
                foreach (var transfer in transfers)
                {
                    transfer.CancelTransfer();
                }
            }
        }

        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            ShowFiles(currentNode, true);
        }
        private void buttonLogout_Click(object sender, RoutedEventArgs e)
        {
            CancelTransfers();
            Invoke(() =>
                {
                    transfers.Clear();

                    while (_mainViewModel.RootNode.Children.Any())
                        _mainViewModel.RootNode.Children.RemoveAt(_mainViewModel.RootNode.Children.Count - 1);
                });
            var userAccount = GetUserKeyFilePath();
            // to restore previous anon account
            //File.Move(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(userAccount), "user.anon.dat"), userAccount);
            // or simply drop logged in account
            File.Delete(userAccount);
            Login(false, userAccount);
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
            transfers.Remove(handle);
        }

        private void buttonFeedback_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://megadesktop.uservoice.com/forums/191321-general");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Process.Start("http://megadesktop.com/");
        }

        private void buttonTrySync_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "MegaSync.exe"));
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

        private void AddDirectoryContent(string path, List<MegaApi.Utility.Tuple<string, string>> list, string root)
        {
            foreach (var file in Directory.GetFiles(path))
            {
                list.Add(new MegaApi.Utility.Tuple<string, string>(file, root));
            }
            foreach (var subdir in Directory.GetDirectories(path))
            {
                list.Add(new MegaApi.Utility.Tuple<string, string>(subdir, root));
                AddDirectoryContent(subdir, list, root);
            }
        }

        public void AddUploadHandle(TransferHandle h)
        {
            Invoke(() => transfers.Add(h));
            h.PropertyChanged += (s, ev) =>
            {
                h.TransferEnded += (s1, e1) => ShowFiles(currentNode, true);
            };

            _mainViewModel.Status.SetStatus(Status.Loaded);
        }
        void AddDownloadHandle(TransferHandle h)
        {
            Invoke(() => transfers.Add(h));
            _mainViewModel.Status.SetStatus(Status.Loaded);
        }
    }
}
