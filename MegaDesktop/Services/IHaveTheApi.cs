using MegaApi;

namespace MegaDesktop.Services
{
    internal interface IHaveTheApi
    {
        void Set(Mega api);

        Mega Api { get; }
    }
}