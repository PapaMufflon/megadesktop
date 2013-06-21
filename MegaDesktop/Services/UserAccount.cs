using System;
using System.IO;
using System.Threading.Tasks;
using MegaApi;
using MegaDesktop.Util;
using MegaDesktop.ViewModels;

namespace MegaDesktop.Services
{
    internal class UserAccount : IUserManagement
    {
        private readonly MegaApiWrapper _megaApiWrapper;
        private readonly RefreshService _refresh;
        private readonly StatusViewModel _status;
        private readonly ICanSetTitle _title;
        private readonly TransferManager _transfers;

        public UserAccount(StatusViewModel status,
                           RefreshService refresh,
                           ICanSetTitle title,
                           TransferManager transfers,
                           MegaApiWrapper megaApiWrapper)
        {
            _status = status.AssertIsNotNull("status");
            _refresh = refresh.AssertIsNotNull("refresh");
            _title = title.AssertIsNotNull("title");
            _transfers = transfers.AssertIsNotNull("transfers");
            _megaApiWrapper = megaApiWrapper.AssertIsNotNull("megaApiWrapper");
        }

        public Task LoginLastUser()
        {
            var userAccountFile = GetUserKeyFilePath();
            var user = Mega.LoadAccount(userAccountFile);

            if (user == null)
                return new Task(() => { });

            return LoginUser(user);
        }

        public Task LoginUser(MegaUser user)
        {
            _status.SetStatus(Status.LoggingIn);

            return Mega.InitAsync(user)
                .ContinueWith(task =>
                {
                    var mega = task.Result;

                    _megaApiWrapper.Register(mega);
                    _transfers.CancelAllTransfers();

                    mega.SaveAccount(GetUserKeyFilePath());

                    var username = mega.User.Status == MegaUserStatus.Anonymous
                                          ? "anonymous account"
                                          : mega.User.Email;

                    _title.SetTitle(Resource.Title + " - " + username);

                    _refresh.Reload();
                    _status.SetStatus(Status.Loaded);
                });
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
            _megaApiWrapper.SaveAccount(GetUserKeyFilePath());
        }

        private string GetUserKeyFilePath()
        {
            var userDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MegaDesktop");

            if (!Directory.Exists(userDirectory))
                Directory.CreateDirectory(userDirectory);

            return Path.Combine(userDirectory, "user.dat");
        }
    }
}