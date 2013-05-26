using System;
using System.IO;
using System.Windows;
using MegaApi;

namespace MegaDesktop.Services
{
    internal class UserAccount : IUserManagement
    {
        private readonly IMegaApi _megaApi;
        private readonly ICanSetStatus _status;
        private readonly ICanRefresh _refresh;
        private readonly ICanSetTitle _title;

        public UserAccount(IMegaApi megaApi, ICanSetStatus status, ICanRefresh refresh, ICanSetTitle title)
        {
            _megaApi = megaApi;
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
                _megaApi.Use(api);

                if (save)
                    SaveAccount(userAccountFile, "user.anon.dat");

                _refresh.Reload();
                _status.SetStatus(Status.Loaded);

                if (_megaApi.User.Status == MegaUserStatus.Anonymous)
                    _title.SetTitle(Resource.Title + " - anonymous account");
                else
                    _title.SetTitle(Resource.Title + " - " + api.User.Email);
            }, e => { MessageBox.Show("Error while loading account: " + e); Application.Current.Shutdown(); });
        }

        public void DeleteCurrentAccount()
        {
            var userAccount = GetUserKeyFilePath();
            // to restore previous anon account
            //File.Move(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(userAccount), "user.anon.dat"), userAccount);
            // or simply drop logged in account
            File.Delete(userAccount);
        }

        public void SaveCurrentAccount()
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

            _megaApi.SaveAccount(GetUserKeyFilePath());
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
