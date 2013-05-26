using System;
using System.Collections.Generic;
using MegaApi;
using MegaApi.DataTypes;
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
            _dispatcher = new TestDispatcher();
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

        [Test]
        public void Update_with_a_node_that_has_the_target_node_as_parent_sets_this_node_as_child_to_the_parent()
        {
            _node.Id = "foo";
            _target.Update(new List<MegaNode> { new MegaNode { ParentId = "foo", Attributes = new NodeAttributes { Name = "bar" } } });

            Assert.That(_target.Children[0].Name, Is.EqualTo("bar"));
        }

        [Test]
        public void Update_a_child_does_not_replace_the_child_node()
        {
            _node.Id = "foo";
            _target.Update(new List<MegaNode> { new MegaNode { ParentId = "foo", Attributes = new NodeAttributes { Name = "bar" } } });

            var child = _target.Children[0];

            _target.Update(new List<MegaNode> { new MegaNode { ParentId = "foo", Attributes = new NodeAttributes { Name = "foo" } } });
            
            Assert.That(child, Is.EqualTo(_target.Children[0]));
            Assert.That(_target.Children[0].Name, Is.EqualTo("foo"));
        }

        [Test]
        public void Update_with_a_child_and_a_grand_child_in_reverse_order_is_possible()
        {
            _node.Id = "foo";
            _target.Update(new List<MegaNode>
            {
                new MegaNode { ParentId = "bar", Id = "baz" },
                new MegaNode { ParentId = "foo", Id = "bar" }
            });

            Assert.That(_target.Children[0].Id, Is.EqualTo("bar"));
            Assert.That(_target.Children[0].Children[0].Id, Is.EqualTo("baz"));
        }
    }
}