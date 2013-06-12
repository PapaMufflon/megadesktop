using System;
using MegaApi;
using MegaDesktop.Commands;
using MegaDesktop.Services;
using MegaDesktop.Services.Fakes;
using MegaDesktop.ViewModels;
using MegaDesktop.ViewModels.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using NSubstitute;
using NUnit.Framework;

namespace MegaDesktop.Tests
{
    [TestFixture]
    public class UploadFolderCommandTests
    {
        [SetUp]
        public void Setup()
        {
            _shimContext = ShimsContext.Create();

            _status = new ShimStatusViewModel();
            _nodes = new ShimNodeManager();
            _nodes.SelectedNodeChangedAddEventHandlerOfEventArgs = handler => _raiseSelectedNodeChanged = handler;
            var dispatcher = new TestDispatcher();
            _megaApiWrapper = new ShimMegaApiWrapper();

            
            _target = new UploadFolderCommand(_status, _nodes, dispatcher, _megaApiWrapper);
        }

        [TearDown]
        public void TearDown()
        {
            _shimContext.Dispose();
        }

        private ShimNodeManager _nodes;
        private ShimStatusViewModel _status;
        private UploadFolderCommand _target;
        private MegaApiWrapper _megaApiWrapper;
        private IDisposable _shimContext;
        private EventHandler<EventArgs> _raiseSelectedNodeChanged;

        [Test]
        public void Can_execute_when_selected_node_is_a_folder()
        {
            _status.CurrentStatusGet = () => Status.Loaded;

            bool actual = _target.CanExecute(new NodeViewModel(new TestDispatcher(), new MegaNode
                {
                    Type = MegaNodeType.Folder
                }));

            Assert.That(actual, Is.True);
        }

        [Test]
        public void Cannot_execute_when_selected_node_is_a_file()
        {
            _status.CurrentStatusGet = () => Status.Loaded;

            bool actual = _target.CanExecute(new NodeViewModel(new TestDispatcher(), new MegaNode
                {
                    Type = MegaNodeType.File
                }));

            Assert.That(actual, Is.False);
        }

        [Test]
        public void Cannot_execute_when_status_is_not_loaded()
        {
            _status.CurrentStatusGet = () => Status.Communicating;

            bool actual = _target.CanExecute(new NodeViewModel(new TestDispatcher(), new MegaNode
                {
                    Type = MegaNodeType.Folder
                }));

            Assert.That(actual, Is.False);
        }

        [Test]
        public void Ctor_arguments_should_not_be_null()
        {
            Assert.Throws<ArgumentNullException>(() => new UploadFolderCommand(null, _nodes, new TestDispatcher(), _megaApiWrapper));
            Assert.Throws<ArgumentNullException>(() => new UploadFolderCommand(_status, null, new TestDispatcher(), _megaApiWrapper));
            Assert.Throws<ArgumentNullException>(() => new UploadFolderCommand(_status, _nodes, null, _megaApiWrapper));
            Assert.Throws<ArgumentNullException>(() => new UploadFolderCommand(_status, _nodes, new TestDispatcher(), null));
        }

        [Test]
        public void Raise_CanExecuteChanged_when_selected_node_changes()
        {
            bool raised = false;

            _target.CanExecuteChanged += (s, e) => raised = true;

            _raiseSelectedNodeChanged(this, EventArgs.Empty);

            Assert.That(raised, Is.True);
        }
    }
}