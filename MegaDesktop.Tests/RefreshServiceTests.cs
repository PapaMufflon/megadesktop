using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaApi;
using MegaDesktop.Services;
using MegaDesktop.Services.Fakes;
using MegaDesktop.ViewModels;
using MegaDesktop.ViewModels.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using NUnit.Framework;

namespace MegaDesktop.Tests
{
    [TestFixture]
    public class RefreshServiceTests
    {
        [SetUp]
        public void Setup()
        {
            _shimContext = ShimsContext.Create();

            _status = new ShimStatusViewModel();
            _nodes = new ShimNodeManager();
            _megaApiWrapper = new ShimMegaApiWrapper();

            _target = new RefreshService(_status, _nodes, _megaApiWrapper);
        }

        [TearDown]
        public void TearDown()
        {
            _shimContext.Dispose();
        }

        private ShimNodeManager _nodes;
        private ShimStatusViewModel _status;
        private RefreshService _target;
        private ShimMegaApiWrapper _megaApiWrapper;
        private IDisposable _shimContext;

        [Test]
        public void Ctor_args_should_not_be_null()
        {
            Assert.Throws<ArgumentNullException>(() => new RefreshService(null, _nodes, _megaApiWrapper));
            Assert.Throws<ArgumentNullException>(() => new RefreshService(_status, null, _megaApiWrapper));
            Assert.Throws<ArgumentNullException>(() => new RefreshService(_status, _nodes, null));
        }

        [Test]
        public void RefreshCurrentNode_sets_the_current_node_as_the_selected_node_after_reload()
        {
            var nodeViewModel = new NodeViewModel(new TestDispatcher(), new MegaNode { Id = "foo" });
            var child = new NodeViewModel(new TestDispatcher(), new MegaNode { Id = "bar" });

            _nodes.RootNodeGet = () => nodeViewModel;
            _nodes.SelectedListNodeGet = () => child;

            NodeViewModel selectedListNode = null;
            _nodes.SelectedListNodeSetNodeViewModel = x => selectedListNode = x;

            _megaApiWrapper.GetNodes = () => Task.Factory.StartNew(() => (IEnumerable<MegaNode>)new List<MegaNode>
            {
                new MegaNode {Id = "foo", Type = MegaNodeType.RootFolder},
                new MegaNode {Id = "bar", ParentId = "foo", Type = MegaNodeType.Trash}
            });

            _target.RefreshCurrentNode();

            AsyncTestsHelper.WaitFor(() => selectedListNode != null && selectedListNode.Type == NodeType.Trash);

            Assert.That(selectedListNode.Type, Is.EqualTo(NodeType.Trash));
        }

        [Test]
        public void Reload_changes_the_status_to_communicating_and_then_to_loaded()
        {
            var setStatus = new List<Status>();
            _status.SetStatusStatus = setStatus.Add;

            var nodeViewModel = new NodeViewModel(new TestDispatcher(), new MegaNode { Id = "foo" });
            _nodes.RootNodeGet = () => nodeViewModel;

            _megaApiWrapper.GetNodes = () => Task.Factory.StartNew(() => (IEnumerable<MegaNode>)new List<MegaNode>
            {
                new MegaNode { ParentId = "foo", Type = MegaNodeType.RootFolder }
            });

            _target.Reload();

            AsyncTestsHelper.WaitFor(() => setStatus.Count == 2);

            Assert.That(setStatus[1], Is.EqualTo(Status.Loaded));
            Assert.That(setStatus[0], Is.EqualTo(Status.Communicating));
        }

        [Test]
        public void Reload_updates_the_RootNode()
        {
            var nodeViewModel = new NodeViewModel(new TestDispatcher(), new MegaNode { Id = "foo" });
            _nodes.RootNodeGet = () => nodeViewModel;

            _megaApiWrapper.GetNodes = () => Task.Factory.StartNew(() => (IEnumerable<MegaNode>)new List<MegaNode>
            {
                new MegaNode { ParentId = "foo", Type = MegaNodeType.RootFolder }
            });

            _target.Reload();

            AsyncTestsHelper.WaitFor(() => nodeViewModel.Children.Count == 1);

            Assert.That(nodeViewModel.Children.Count, Is.EqualTo(1));
        }
    }
}