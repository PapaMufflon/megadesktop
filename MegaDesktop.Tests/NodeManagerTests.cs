using System;
using MegaDesktop.Commands;
using MegaDesktop.Commands.Fakes;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;
using NUnit.Framework;

namespace MegaDesktop.Tests
{
    [TestFixture]
    public class NodeManagerTests
    {
        private NodeManager _target;
        private DownloadCommand _downloadCommand;

        [SetUp]
        public void Setup()
        {
            _downloadCommand = new ShimDownloadCommand();
            _target = new NodeManager(new TestDispatcher(), _downloadCommand);
        }

        [Test]
        public void Ctor_arguments_should_not_be_null()
        {
            Assert.Throws<ArgumentNullException>(() => new NodeManager(null, _downloadCommand));
            Assert.Throws<ArgumentNullException>(() => new NodeManager(new TestDispatcher(), null));
        }

        [Test]
        public void Selecting_a_node_raises_the_SelectedNodeChanged_event()
        {
            bool raised = false;
            _target.SelectedNodeChanged += (s, e) => raised = true;

            _target.SelectedListNode = new NodeViewModel(new TestDispatcher());

            Assert.That(raised, Is.True);

            raised = false;

            _target.SelectedTreeNode = new NodeViewModel(new TestDispatcher());

            Assert.That(raised, Is.True);
        }
    }
}
