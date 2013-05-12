using System;
using System.IO;
using System.Windows;
using MegaApi;

namespace MegaDesktop.Services
{
    internal class UserAccount : IUserManagement
    {
        private readonly IHaveTheApi _apiManager;
        private readonly ICanSetStatus _status;
        private readonly ICanRefresh _refresh;
        private readonly ICanSetTitle _title;

        public UserAccount(IHaveTheApi apiManager, ICanSetStatus status, ICanRefresh refresh, ICanSetTitle title)
        {
            _apiManager = apiManager;
            _status = status;
            _refresh = refresh;
            _title = title;
        }

        public void AutoLoginLastUser()
        {
            var userAccountFile = GetUserKeyFilePath();
            var save = false;
            MegaUser user;
            
            if ((user = Mega.LoadAccount(userAccountFile)) == null)
                save = true;

            _status.SetStatus(Status.LoggingIn);

            Mega.Init(user, api =>
            {
                _apiManager.Set(api);

                if (save)
                    SaveAccount(userAccountFile, "user.anon.dat");

                _refresh.Reload();
                _status.SetStatus(Status.Loaded);

                if (_apiManager.Api.User.Status == MegaUserStatus.Anonymous)
                    _title.Title = Resource.Title + " - anonymous account";
                else
                    _title.Title = Resource.Title + " - " + api.User.Email;
            }, e => { MessageBox.Show("Error while loading account: " + e); Application.Current.Shutdown(); });
        }

        public void SaveAccount()
        {
            SaveAccount(GetUserKeyFilePath(), "user.anon.dat");
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

            _apiManager.Api.SaveAccount(GetUserKeyFilePath());
        }

        private string GetUserKeyFilePath()
        {
            string userDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            userDir += System.IO.Path.DirectorySeparatorChar + "MegaDesktop";
            if (!Directory.Exists(userDir)) { Directory.CreateDirectory(userDir); }
            userDir += System.IO.Path.DirectorySeparatorChar;
            return userDir + "user.dat";
        }
    }
}
