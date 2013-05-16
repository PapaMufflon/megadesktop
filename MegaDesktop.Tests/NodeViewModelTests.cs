using System;
using MegaApi;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;
using NSubstitute;
using NUnit.Framework;

namespace MegaDesktop.Tests
{
    [TestFixture]
    public class NodeViewModelTests
    {
        private IDispatcher _dispatcher;
        private MegaNode _node;
        private NodeViewModel _target;

        [SetUp]
        public void Setup()
        {
            _dispatcher = Substitute.For<IDispatcher>();
            _node = new MegaNode();

            _target = new NodeViewModel(_dispatcher, _node);
        }

        [Test]
        public void Ctor_arguments_should_not_be_null()
        {
            Assert.Throws<ArgumentNullException>(() => new NodeViewModel(null));
            Assert.Throws<ArgumentNullException>(() => new NodeViewModel(null, _node));
            Assert.Throws<ArgumentNullException>(() => new NodeViewModel(_dispatcher, null));
        }
    }
}