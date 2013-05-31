using System;
using MegaApi;
using MegaDesktop.Commands;
using MegaDesktop.Services;
using MegaDesktop.ViewModels;
using NSubstitute;
using NUnit.Framework;

namespace MegaDesktop.Tests
{
    [TestFixture]
    public class UploadFolderCommandTests
    {
        private IHaveNodes _nodes;
        private ISelectedNodeListener _listener;
        private ICanSetStatus _status;
        private IMegaApi _megaApi;
        private UploadFolderCommand _target;

        [SetUp]
        public void Setup()
        {
            _megaApi = Substitute.For<IMegaApi>();
            _status = Substitute.For<ICanSetStatus>();
            _listener = Substitute.For<ISelectedNodeListener>();
            _nodes = Substitute.For<IHaveNodes>();
            var dispatcher = new TestDispatcher();

            _target = new UploadFolderCommand(_megaApi, _status, _listener, _nodes, dispatcher);
        }

        [Test]
        public void Ctor_arguments_should_not_be_null()
        {
            Assert.Throws<ArgumentNullException>(() => new UploadFolderCommand(null, _status, _listener, _nodes, new TestDispatcher()));
            Assert.Throws<ArgumentNullException>(() => new UploadFolderCommand(_megaApi, null, _listener, _nodes, new TestDispatcher()));
            Assert.Throws<ArgumentNullException>(() => new UploadFolderCommand(_megaApi, _status, null, _nodes, new TestDispatcher()));
            Assert.Throws<ArgumentNullException>(() => new UploadFolderCommand(_megaApi, _status, _listener, null, new TestDispatcher()));
            Assert.Throws<ArgumentNullException>(() => new UploadFolderCommand(_megaApi, _status, _listener, _nodes, null));
        }

        [Test]
        public void Cannot_execute_when_selected_node_is_a_file()
        {
            _status.CurrentStatus.Returns(Status.Loaded);

            var actual = _target.CanExecute(new NodeViewModel(new TestDispatcher(), new MegaNode
            {
                Type = MegaNodeType.File
            }));

            Assert.That(actual, Is.False);
        }

        [Test]
        public void Can_execute_when_selected_node_is_a_folder()
        {
            _status.CurrentStatus.Returns(Status.Loaded);

            var actual = _target.CanExecute(new NodeViewModel(new TestDispatcher(), new MegaNode
            {
                Type = MegaNodeType.Folder
            }));

            Assert.That(actual, Is.True);
        }

        [Test]
        public void Cannot_execute_when_status_is_not_loaded()
        {
            _status.CurrentStatus.Returns(Status.Communicating);

            var actual = _target.CanExecute(new NodeViewModel(new TestDispatcher(), new MegaNode
            {
                Type = MegaNodeType.Folder
            }));

            Assert.That(actual, Is.False);
        }

        [Test]
        public void 
    }
}