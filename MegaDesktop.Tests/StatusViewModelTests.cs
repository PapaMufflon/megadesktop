using MegaDesktop.Services;
using MegaDesktop.ViewModels;
using NUnit.Framework;

namespace MegaDesktop.Tests
{
    [TestFixture]
    public class StatusViewModelTests
    {
        [Test]
        public void Changing_a_status_changes_also_the_message()
        {
            var target = new StatusViewModel(new TestDispatcher());

            target.SetStatus(Status.Loaded);

            string message = target.Message;

            target.SetStatus(Status.Communicating);

            Assert.That(message, Is.Not.EqualTo(target.Message));
        }

        [Test]
        public void Changing_the_status_raises_the_CurrentStatusChanged_event()
        {
            bool raised = false;
            var target = new StatusViewModel(new TestDispatcher());

            target.CurrentStatusChanged += (s, e) => raised = true;

            target.SetStatus(Status.Loaded);

            Assert.That(raised, Is.True);
        }

        [Test]
        public void Setting_a_status_makes_that_the_current_status()
        {
            var target = new StatusViewModel(new TestDispatcher());

            target.SetStatus(Status.Loaded);

            Assert.That(target.CurrentStatus, Is.EqualTo(Status.Loaded));
        }
    }
}