using MegaApi;

namespace MegaDesktop.Services
{
    internal class ApiManager : IHaveTheApi
    {
        private Mega _api;

        public void Set(Mega api)
        {
            _api = api;
        }

        public Mega Api { get { return _api; } }
    }
}