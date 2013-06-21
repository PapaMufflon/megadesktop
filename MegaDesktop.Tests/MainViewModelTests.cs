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
            
            _status = new ShimStatusViewModel();
            _nodeManager = new ShimNodeManager();
            _transfersViewModel = new ShimTransfersViewModel();

            _target = new MainViewModel(_status, _nodeManager, _transfersViewModel);
        }

        [TearDown]
        public void TearDown()
        {
            _shimContext.Dispose();
        }

        private MainViewModel _target;
        private StatusViewModel _status;
        private NodeManager _nodeManager;
        private IDisposable _shimContext;
        private TransfersViewModel _transfersViewModel;

        [Test]
        public void Ctor_arguments_should_not_be_null()
        {
            Assert.Throws<ArgumentNullException>(() => new MainViewModel(null, _nodeManager, _transfersViewModel));
            Assert.Throws<ArgumentNullException>(() => new MainViewModel(_status, null, _transfersViewModel));
            Assert.Throws<ArgumentNullException>(() => new MainViewModel(_status, _nodeManager, null));
        }

        [Test]
        public void Selecting_a_node_in_the_list_sets_this_node_as_the_selected_node()
        {
            _target.SelectedListNode = new NodeViewModel(new TestDispatcher());

            Assert.That(_target.SelectedNode, Is.EqualTo(_target.SelectedListNode));
        }

        [Test]
        public void Selecting_a_node_in_the_tree_sets_this_node_as_the_selected_node()
        {
            _target.SelectedTreeNode = new NodeViewModel(new TestDispatcher());

            Assert.That(_target.SelectedNode, Is.EqualTo(_target.SelectedTreeNode));
        }
    }
}