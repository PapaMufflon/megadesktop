using System;
using MegaDesktop.Services;
using NSubstitute;
using NUnit.Framework;

namespace MegaDesktop.Tests
{
    [TestFixture]
    public class UserAccountTests
    {
        private ICanSetTitle _title;
        private ICanRefresh _refresh;
        private ICanSetStatus _status;
        private IMegaApi _megaApi;
        private IManageTransfers _transfers;
        private UserAccount _target;

        [SetUp]
        public void Setup()
        {
            _megaApi = Substitute.For<IMegaApi>();
            _status = Substitute.For<ICanSetStatus>();
            _refresh = Substitute.For<ICanRefresh>();
            _title = Substitute.For<ICanSetTitle>();
            _transfers = Substitute.For<IManageTransfers>();

            _target = new UserAccount(_megaApi, _status, _refresh, _title, _transfers);
        }

        [Test]
        public void Ctor_arguments_should_not_be_null()
        {
            Assert.Throws<ArgumentNullException>(() => new UserAccount(null, _status, _refresh, _title, _transfers));
            Assert.Throws<ArgumentNullException>(() => new UserAccount(_megaApi, null, _refresh, _title, _transfers));
            Assert.Throws<ArgumentNullException>(() => new UserAccount(_megaApi, _status, null, _title, _transfers));
            Assert.Throws<ArgumentNullException>(() => new UserAccount(_megaApi, _status, _refresh, null, _transfers));
            Assert.Throws<ArgumentNullException>(() => new UserAccount(_megaApi, _status, _refresh, _title, null));
        }

        [Test]
        public void If_there_is_an_error_when_logging_in_Then_the_task_should_have_an_exception()
        {
            // todo
        }
    }
}