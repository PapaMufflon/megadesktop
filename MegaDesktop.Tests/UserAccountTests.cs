using System;
using MegaDesktop.Services;
using MegaDesktop.Services.Fakes;
using MegaDesktop.ViewModels;
using MegaDesktop.ViewModels.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using NSubstitute;
using NUnit.Framework;

namespace MegaDesktop.Tests
{
    [TestFixture]
    public class UserAccountTests
    {
        [SetUp]
        public void Setup()
        {
            _shimContext = ShimsContext.Create();

            _status = new ShimStatusViewModel();
            _refresh = new ShimRefreshService();
            _title = Substitute.For<ICanSetTitle>();
            _transfers = new ShimTransferManager();
            _megaApiWrapper = new ShimMegaApiWrapper();

            _target = new UserAccount(_status, _refresh, _title, _transfers, _megaApiWrapper);
        }

        [TearDown]
        public void TearDown()
        {
            _shimContext.Dispose();
        }

        private ICanSetTitle _title;
        private StatusViewModel _status;
        private TransferManager _transfers;
        private UserAccount _target;
        private RefreshService _refresh;
        private MegaApiWrapper _megaApiWrapper;
        private IDisposable _shimContext;

        [Test]
        public void Ctor_arguments_should_not_be_null()
        {
            Assert.Throws<ArgumentNullException>(() => new UserAccount(null, _refresh, _title, _transfers, _megaApiWrapper));
            Assert.Throws<ArgumentNullException>(() => new UserAccount(_status, null, _title, _transfers, _megaApiWrapper));
            Assert.Throws<ArgumentNullException>(() => new UserAccount(_status, _refresh, null, _transfers, _megaApiWrapper));
            Assert.Throws<ArgumentNullException>(() => new UserAccount(_status, _refresh, _title, null, _megaApiWrapper));
            Assert.Throws<ArgumentNullException>(() => new UserAccount(_status, _refresh, _title, _transfers, null));
        }

        [Test]
        public void If_there_is_an_error_when_logging_in_Then_the_task_should_have_an_exception()
        {
            // todo
        }
    }
}