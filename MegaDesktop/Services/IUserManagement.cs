namespace MegaDesktop.Services
{
    internal interface IUserManagement
    {
        void SaveCurrentAccount();
        void DeleteCurrentAccount();
        void AutoLoginLastUser();
    }
}