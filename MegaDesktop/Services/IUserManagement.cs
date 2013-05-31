using System.Threading.Tasks;
using MegaApi;

namespace MegaDesktop.Services
{
    internal interface IUserManagement
    {
        void SaveCurrentAccount();
        void DeleteCurrentAccount();
        Task LoginLastUser();
        Task LoginUser(MegaUser user);
    }
}