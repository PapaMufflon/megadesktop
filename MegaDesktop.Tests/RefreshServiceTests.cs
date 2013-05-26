using System;
using System.Collections.Generic;
using System.Linq;
using MegaApi;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;
using NSubstitute;
using NUnit.Framework;

namespace MegaDesktop.Tests
{
    [TestFixture]
    public class RefreshServiceTests
    {
        private IHaveNodes _nodes;
        private IMegaApi _megaApi;
        private ICanSetStatus _status;
        private RefreshService _target;

        [SetUp]
        public void Setup()
        {
            _status = Substitute.For<ICanSetStatus>();
            _megaApi = Substitute.For<IMegaApi>();
            _nodes = Substitute.For<IHaveNodes>();

            _target = new RefreshService(_status, _megaApi, _nodes);
        }

        [Test]
        public void Ctor_args_should_not_be_null()
        {
            Assert.Throws<ArgumentNullException>(() => new RefreshService(null, _megaApi, _nodes));
            Assert.Throws<ArgumentNullException>(() => new RefreshService(_status, null, _nodes));
            Assert.Throws<ArgumentNullException>(() => new RefreshService(_status, _megaApi, null));
        }

        [Test]
        public void Reload_updates_the_RootNode()
        {
            var nodeViewModel = new NodeViewModel(new TestDispatcher(), new MegaNode { Id = "foo" });
            _nodes.RootNode.Returns(nodeViewModel);

            _megaApi.When(x => x.GetNodes(Arg.Any<Action<List<MegaNode>>>(), Arg.Any<Action<int>>()))
                    .Do(x => ((Action<List<MegaNode>>)x.Args()[0])
                        .Invoke(new List<MegaNode> { new MegaNode { Id = "foo", Type = MegaNodeType.RootFolder } }));

            _target.Reload();

            Assert.That(nodeViewModel.Children.Count, Is.EqualTo(1));
        }

        [Test]
        public void Reload_changes_the_status_to_communicating_and_then_to_loaded()
        {
            var nodeViewModel = new NodeViewModel(new TestDispatcher(), new MegaNode { Id = "foo" });
            _nodes.RootNode.Returns(nodeViewModel);

            _megaApi.When(x => x.GetNodes(Arg.Any<Action<List<MegaNode>>>(), Arg.Any<Action<int>>()))
                    .Do(x => ((Action<List<MegaNode>>)x.Args()[0])
                        .Invoke(new List<MegaNode> { new MegaNode { Id = "foo", Type = MegaNodeType.RootFolder } }));

            _target.Reload();

            _status.Received().SetStatus(Status.Loaded);
            _status.Received().SetStatus(Status.Communicating);
        }

        [Test]
        public void RefreshCurrentNode_sets_the_current_node_as_the_selected_node_after_reload()
        {
            var nodeViewModel = new NodeViewModel(new TestDispatcher(), new MegaNode { Id = "foo" });
            var child = new NodeViewModel(new TestDispatcher(), new MegaNode { Id = "bar" });

            _nodes.RootNode.Returns(nodeViewModel);
            _nodes.SelectedListNode = child;

            _megaApi.When(x => x.GetNodes(Arg.Any<Action<List<MegaNode>>>(), Arg.Any<Action<int>>()))
                    .Do(x => ((Action<List<MegaNode>>)x.Args()[0])
                        .Invoke(new List<MegaNode>
                            {
                                new MegaNode {Id = "foo", Type = MegaNodeType.RootFolder },
                                new MegaNode {Id = "bar", ParentId = "foo", Type = MegaNodeType.Trash }
                            }));

            _target.RefreshCurrentNode();

            Assert.That(_nodes.SelectedListNode.Type, Is.EqualTo(NodeType.Trash));
        }
    }
}