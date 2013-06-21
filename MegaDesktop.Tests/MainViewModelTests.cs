using System;
using MegaDesktop.Services;
using MegaDesktop.Services.Fakes;
using MegaDesktop.ViewModels;
using MegaDesktop.ViewModels.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using NUnit.Framework;

namespace MegaDesktop.Tests
{
    [TestFixture]
    public class MainViewModelTests
    {
        [SetUp]
        public void Setup()
        {
            _shimContext = ShimsContext.Create();
            
            _nodeManager = new ShimNodeManager();
            _transfersViewModel = new ShimTransfersViewModel();
            _toolBarViewModel = new ShimToolBarViewModel();
            _headerViewModel = new ShimHeaderViewModel();

            _target = new MainViewModel(_nodeManager, _transfersViewModel, _toolBarViewModel, _headerViewModel);
        }

        [TearDown]
        public void TearDown()
        {
            _shimContext.Dispose();
        }

        private MainViewModel _target;
        private NodeManager _nodeManager;
        private IDisposable _shimContext;
        private TransfersViewModel _transfersViewModel;
        private ToolBarViewModel _toolBarViewModel;
        private HeaderViewModel _headerViewModel;

        [Test]
        public void Ctor_arguments_should_not_be_null()
        {
            Assert.Throws<ArgumentNullException>(() => new MainViewModel(null, _transfersViewModel, _toolBarViewModel, _headerViewModel));
            Assert.Throws<ArgumentNullException>(() => new MainViewModel(_nodeManager, null, _toolBarViewModel, _headerViewModel));
            Assert.Throws<ArgumentNullException>(() => new MainViewModel(_nodeManager, _transfersViewModel, null, _headerViewModel));
            Assert.Throws<ArgumentNullException>(() => new MainViewModel(_nodeManager, _transfersViewModel, _toolBarViewModel, null));
        }
    }
}