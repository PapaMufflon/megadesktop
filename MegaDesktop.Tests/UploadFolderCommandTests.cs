using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Fakes;
using System.Windows.Forms;
using System.Windows.Forms.Fakes;
using MegaApi;
using MegaDesktop.Commands;
using MegaDesktop.Commands.Fakes;
using MegaDesktop.Services;
using MegaDesktop.Services.Fakes;
using MegaDesktop.ViewModels;
using MegaDesktop.ViewModels.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
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
            _uploadCommand = new ShimUploadCommand();

            _target = new UploadFolderCommand(_status, _nodes, dispatcher, _megaApiWrapper, _uploadCommand);
        }

        [TearDown]
        public void TearDown()
        {
            _shimContext.Dispose();
        }

        private ShimNodeManager _nodes;
        private ShimStatusViewModel _status;
        private UploadFolderCommand _target;
        private ShimMegaApiWrapper _megaApiWrapper;
        private IDisposable _shimContext;
        private EventHandler<EventArgs> _raiseSelectedNodeChanged;
        private ShimUploadCommand _uploadCommand;

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
            Assert.Throws<ArgumentNullException>(() => new UploadFolderCommand(null, _nodes, new TestDispatcher(), _megaApiWrapper, _uploadCommand));
            Assert.Throws<ArgumentNullException>(() => new UploadFolderCommand(_status, null, new TestDispatcher(), _megaApiWrapper, _uploadCommand));
            Assert.Throws<ArgumentNullException>(() => new UploadFolderCommand(_status, _nodes, null, _megaApiWrapper, _uploadCommand));
            Assert.Throws<ArgumentNullException>(() => new UploadFolderCommand(_status, _nodes, new TestDispatcher(), null, _uploadCommand));
            Assert.Throws<ArgumentNullException>(() => new UploadFolderCommand(_status, _nodes, new TestDispatcher(), _megaApiWrapper, null));
        }

        [Test]
        public void Raise_CanExecuteChanged_when_selected_node_changes()
        {
            bool raised = false;

            _target.CanExecuteChanged += (s, e) => raised = true;

            _raiseSelectedNodeChanged(this, EventArgs.Empty);

            Assert.That(raised, Is.True);
        }

        [Test]
        public void When_no_source_folder_is_chosen_Then_do_nothing()
        {
            ShimCommonDialog.AllInstances.ShowDialog = x => DialogResult.Cancel;

            var status = Status.Loaded;
            _status.SetStatusStatus = x => status = x;

            _target.Execute(new NodeViewModel(new TestDispatcher()));

            Assert.That(status, Is.EqualTo(Status.Loaded));
        }

        [Test]
        public void Uploading_only_a_new_folder_Adds_the_folder_to_the_current_node()
        {
            var called = false;

            ShimCommonDialog.AllInstances.ShowDialog = x => DialogResult.OK;
            _megaApiWrapper.CreateFolderStringStringActionOfMegaNodeActionOfInt32 = (s, s1, arg3, arg4) => called = true;

            _target.Execute(new NodeViewModel(new TestDispatcher()));

            Assert.That(called, Is.True);
        }

        [Test]
        public void Uploading_a_new_folder_with_files_in_it_Uploads_the_files_as_soon_as_the_folder_was_created()
        {
            var called = 0;
            var parentId = "1";

            ShimCommonDialog.AllInstances.ShowDialog = x => DialogResult.OK;
            ShimDirectory.EnumerateFilesString = s => new[] { "foo", "bar" };
            ShimDirectory.EnumerateDirectoriesString = s => new List<string>();

            var nodeViewModel = new NodeViewModel(new TestDispatcher());
            nodeViewModel.Children.Add(new NodeViewModel(new TestDispatcher(), new MegaNode { Id = parentId }));
            _nodes.RootNodeGet = () => nodeViewModel;

            _megaApiWrapper.CreateFolderStringStringActionOfMegaNodeActionOfInt32 = (s, s1, callback, arg4) => callback(new MegaNode { ParentId = parentId });
            _uploadCommand.UploadFileStringNodeViewModel = (s, model) => called++;

            _target.Execute(new NodeViewModel(new TestDispatcher()));

            Assert.That(called, Is.EqualTo(2));
        }

        [Test]
        public void Uploading_a_new_folder_with_folders_in_it_Uploads_the_folders_as_soon_as_the_root_folder_was_created()
        {
            var called = 0;
            var parentId = "1";
            var first = true;

            ShimCommonDialog.AllInstances.ShowDialog = x => DialogResult.OK;
            ShimDirectory.EnumerateFilesString = s => new List<string>();
            ShimDirectory.EnumerateDirectoriesString = s =>
            {
                if (!first)
                    return new List<string>();

                first = false;
                return new[] {"C:\\foo", "C:\\bar"};
            };

            var nodeViewModel = new NodeViewModel(new TestDispatcher());
            nodeViewModel.Children.Add(new NodeViewModel(new TestDispatcher(), new MegaNode { Id = parentId }));
            _nodes.RootNodeGet = () => nodeViewModel;

            _megaApiWrapper.CreateFolderStringStringActionOfMegaNodeActionOfInt32 = (s, s1, callback, arg4) =>
            {
                callback(new MegaNode { ParentId = parentId });
                called++;
            };

            _target.Execute(new NodeViewModel(new TestDispatcher()));

            Assert.That(called, Is.EqualTo(3));
        }
    }
}