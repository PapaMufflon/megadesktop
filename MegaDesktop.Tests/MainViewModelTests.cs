using System;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;
using NSubstitute;
using NUnit.Framework;

namespace MegaDesktop.Tests
{
    [TestFixture]
    public class MainViewModelTests
    {
        private ICanSetTitle _title;
        private IDispatcher _dispatcher;
        private MainViewModel _target;

        [SetUp]
        public void Setup()
        {
            _dispatcher = new TestDispatcher();
            _title = Substitute.For<ICanSetTitle>();

            _target = new MainViewModel(_dispatcher, _title);
        }

        [Test]
        public void Ctor_arguments_should_not_be_null()
        {
            Assert.Throws<ArgumentNullException>(() => new MainViewModel(null, _title));
            Assert.Throws<ArgumentNullException>(() => new MainViewModel(_dispatcher, null));
        }
    }
}