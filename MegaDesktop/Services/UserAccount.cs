using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MegaApi;

namespace MegaDesktop.Services
{
    internal class UserAccount : IUserManagement
    {
        private readonly IManageTransfers _transfers;
        private readonly IMegaApi _megaApi;
        private readonly ICanSetStatus _status;
        private readonly ICanRefresh _refresh;
        private readonly ICanSetTitle _title;

        public UserAccount(IMegaApi megaApi, ICanSetStatus status, ICanRefresh refresh, ICanSetTitle title, IManageTransfers transfers)
        {
            _megaApi = megaApi.AssertIsNotNull("megaApi");
            _status = status.AssertIsNotNull("status");
            _refresh = refresh.AssertIsNotNull("refresh");
            _title = title.AssertIsNotNull("title");
            _transfers = transfers;
        }

        public Task LoginLastUser()
        {
            var userAccountFile = GetUserKeyFilePath();
            var user = _megaApi.LoadAccount(userAccountFile);

            return LoginUser(user);
        }

        public Task LoginUser(MegaUser user)
        {
            _status.SetStatus(Status.LoggingIn);

            var waitHandle = new AutoResetEvent(false);
            var cancellationSource = new CancellationTokenSource();
            var token = cancellationSource.Token;

            var task = Task.Factory.StartNew(handle =>
            {
                ((AutoResetEvent)handle).WaitOne();

                token.ThrowIfCancellationRequested();
            }, waitHandle, token);

            _megaApi.Init(user, () =>
            {
                _transfers.CancelAllTransfers();

                _megaApi.SaveAccount(GetUserKeyFilePath());

                var username = _megaApi.User.Status == MegaUserStatus.Anonymous
                                   ? "anonymous account"
                                   : _megaApi.User.Email;

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
            var userAccount = GetUserKeyFilePath();
            // to restore previous anon account
            //File.Move(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(userAccount), "user.anon.dat"), userAccount);
            // or simply drop logged in account
            File.Delete(userAccount);
        }

        public void SaveCurrentAccount()
        {
            _megaApi.SaveAccount(GetUserKeyFilePath());
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