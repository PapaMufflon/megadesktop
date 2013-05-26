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

        [Test]
        public void Selecting_a_node_in_the_tree_sets_this_node_as_the_selected_node()
        {
            _target.SelectedTreeNode = new NodeViewModel(_dispatcher);

            Assert.That(_target.SelectedNode, Is.EqualTo(_target.SelectedTreeNode));
        }

        [Test]
        public void Selecting_a_node_in_the_list_sets_this_node_as_the_selected_node()
        {
            _target.SelectedListNode = new NodeViewModel(_dispatcher);

            Assert.That(_target.SelectedNode, Is.EqualTo(_target.SelectedListNode));
        }

        [Test]
        public void Selecting_a_node_raises_the_SelectedNodeChanged_event()
        {
            var raised = false;
            _target.SelectedNodeChanged += (s, e) => raised = true;

            _target.SelectedListNode = new NodeViewModel(_dispatcher);

            Assert.That(raised, Is.True);

            raised = false;

            _target.SelectedTreeNode = new NodeViewModel(_dispatcher);

            Assert.That(raised, Is.True);
        }
    }
}