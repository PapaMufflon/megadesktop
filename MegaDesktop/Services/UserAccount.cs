using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MegaApi;
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
            string userAccountFile = GetUserKeyFilePath();
            MegaUser user = Mega.LoadAccount(userAccountFile);

            return LoginUser(user);
        }

        public Task LoginUser(MegaUser user)
        {
            _status.SetStatus(Status.LoggingIn);

            var waitHandle = new AutoResetEvent(false);
            var cancellationSource = new CancellationTokenSource();
            CancellationToken token = cancellationSource.Token;

            Task task = Task.Factory.StartNew(handle =>
                {
                    ((AutoResetEvent)handle).WaitOne();

                    token.ThrowIfCancellationRequested();
                }, waitHandle, token);

            Mega.Init(user, mega =>
                {
                    _megaApiWrapper.Register(mega);
                    _transfers.CancelAllTransfers();

                    mega.SaveAccount(GetUserKeyFilePath());

                    string username = mega.User.Status == MegaUserStatus.Anonymous
                                          ? "anonymous account"
                                          : mega.User.Email;

                    _title.SetTitle(Resource.Title + " - " + username);

                    _refresh.Reload();
                    _status.SetStatus(Status.Loaded);

                    waitHandle.Set();
                }, e =>
                    {
                        cancellationSource.Cancel();
                        waitHandle.Set();
                    });

            return task;
        }

        public void DeleteCurrentAccount()
        {
            string userAccount = GetUserKeyFilePath();
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
            string userDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MegaDesktop");

            if (!Directory.Exists(userDirectory))
                Directory.CreateDirectory(userDirectory);

            return Path.Combine(userDirectory, "user.dat");
        }
    }
}